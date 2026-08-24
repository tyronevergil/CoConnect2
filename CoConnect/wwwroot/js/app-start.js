(function (app, $) {

	var connectionId = null;

	let _lastTimestamp = 0;
	let _sequence = 0;

	function formatGuid(bytes) {
		const hex = [...bytes]
			.map(b => b.toString(16).padStart(2, "0"))
			.join("");

		return (
			hex.substring(0, 8) + "-" +
			hex.substring(8, 12) + "-" +
			hex.substring(12, 16) + "-" +
			hex.substring(16, 20) + "-" +
			hex.substring(20)
		);
	}

	function handleAjaxError(xhr, status, error, operation) {
		console.error('AJAX Failed!');
		console.error('Operation:', operation);
		console.error('Status:', status);
		console.error('Error:', error);
		console.error('XHR Status Code:', xhr.status);

		let errorMsg = 'Failed to ' + operation + '. ';
		if (status === 'timeout') {
			errorMsg += 'Server is not responding.';
		} else if (status === 'error' && xhr.status === 0) {
			errorMsg += 'Cannot connect to server.';
		} else {
			errorMsg += 'Please try again.';
		}

		app.showToast(errorMsg, 'error');
	}

	app.newSequentialGuid = function() {
		const bytes = new Uint8Array(16);
		crypto.getRandomValues(bytes);

		let timestamp = Date.now();

		// Monotonic within the same millisecond
		if (timestamp === _lastTimestamp) {
			_sequence = (_sequence + 1) & 0xffff;
		} else {
			_lastTimestamp = timestamp;
			_sequence = 0;
		}

		// 48-bit timestamp (big-endian)
		bytes[0] = (timestamp >>> 40) & 0xff;
		bytes[1] = (timestamp >>> 32) & 0xff;
		bytes[2] = (timestamp >>> 24) & 0xff;
		bytes[3] = (timestamp >>> 16) & 0xff;
		bytes[4] = (timestamp >>> 8) & 0xff;
		bytes[5] = timestamp & 0xff;

		// 16-bit sequence
		bytes[6] = (_sequence >>> 8) & 0xff;
		bytes[7] = _sequence & 0xff;

		// Remaining 8 bytes stay random

		// UUID variant
		bytes[8] = (bytes[8] & 0x3f) | 0x80;

		return formatGuid(bytes);
	}

	app.showToast = function(message, type) {
		let toastEl = document.getElementById('notificationToast');
		let toastBody = toastEl.querySelector('.toast-body');
		let closeBtn = toastEl.querySelector('.btn-close');

		// Set message
		toastBody.textContent = message;

		// Remove all previous color classes
		toastEl.className = 'toast align-items-center';
		closeBtn.className = 'btn-close me-2 m-auto';

		// Add styling based on type
		if (type === 'success') {
			toastEl.classList.add('bg-success', 'text-white');
			closeBtn.classList.add('btn-close-white');
		} else if (type === 'error') {
			toastEl.classList.add('bg-danger', 'text-white');
			closeBtn.classList.add('btn-close-white');
		} else if (type === 'warning') {
			toastEl.classList.add('bg-warning', 'text-dark');
			closeBtn.classList.add('btn-close-white');
		} else if (type === 'info') {
			toastEl.classList.add('bg-info', 'text-dark');
			closeBtn.classList.add('btn-close-white');
		} else {
			toastEl.classList.add('bg-light', 'text-dark');
		}

		// Show toast
		let toast = new bootstrap.Toast(toastEl, {
			autohide: true,
			delay: 5000
		});
		toast.show();
	}

	app.showConfirm = function (options) {
		var modalEl = document.getElementById('appConfirmModal');
		if (!modalEl) {
			return Promise.resolve(window.confirm((options && options.message) || 'Are you sure?'));
		}

		var titleEl = document.getElementById('appConfirmTitle');
		var messageEl = document.getElementById('appConfirmMessage');
		var okBtn = document.getElementById('appConfirmOkBtn');
		var cancelBtn = document.getElementById('appConfirmCancelBtn');

		titleEl.textContent = (options && options.title) || 'Confirm';
		messageEl.textContent = (options && options.message) || 'Are you sure?';
		okBtn.textContent = (options && options.okText) || 'Confirm';
		cancelBtn.textContent = (options && options.cancelText) || 'Cancel';

		okBtn.className = 'btn ' + ((options && options.okButtonClass) || 'btn-primary');

		var modal = bootstrap.Modal.getOrCreateInstance(modalEl);

		return new Promise(function (resolve) {
			var settled = false;

			function cleanup() {
				okBtn.removeEventListener('click', onOk);
				modalEl.removeEventListener('hide.bs.modal', onHide);
				modalEl.removeEventListener('hidden.bs.modal', onHidden);
			}

			function onHide() {
				if (modalEl.contains(document.activeElement) && typeof document.activeElement.blur === 'function') {
					document.activeElement.blur();
				}
			}

			function onOk() {
				if (settled) return;
				settled = true;
				cleanup();
				resolve(true);
				onHide();
				modal.hide();
			}

			function onHidden() {
				if (settled) return;
				settled = true;
				cleanup();
				resolve(false);
			}

			okBtn.addEventListener('click', onOk);
			modalEl.addEventListener('hide.bs.modal', onHide);
			modalEl.addEventListener('hidden.bs.modal', onHidden, { once: true });
			modal.show();
		});
	}

	app.get = function (operation, url, data) {
		return $.ajax({
			url: url,
			type: 'GET',
			data: data
		})
		.fail(function (xhr, status, error) {
			handleAjaxError(xhr, status, error, operation);
		});
	}

	app.post = function (operation, url, data) {

		data.connectionId = connectionId;
		data.transactionId = app.newSequentialGuid();

		return $.ajax({
			url: url,
			type: 'POST',
			contentType: 'application/json',
			/*timeout: 2500,*/
			data: JSON.stringify(data)
		})
		.fail(function (xhr, status, error) {
			handleAjaxError(xhr, status, error, operation);
		});
	}

	function handleForcedSignout(data) {
		var currentUsername = (app.currentUsername || '').toLowerCase();
		var eventUsername = ((data && data.username) || '').toLowerCase();

		if (!currentUsername || !eventUsername || currentUsername !== eventUsername) {
			return;
		}

		var modalEl = document.getElementById('appSignoutModal');
		var messageEl = document.getElementById('appSignoutMessage');
		var okBtn = document.getElementById('appSignoutOkBtn');
		var redirectUrl = (data && data.redirectUrl) || '/Account/Login';
		var message = (data && data.message) || 'Your session has changed. You will be redirected to sign in again.';

		if (!modalEl || !messageEl || !okBtn) {
			window.location.href = redirectUrl;
			return;
		}

		messageEl.textContent = message;
		var modal = bootstrap.Modal.getOrCreateInstance(modalEl);

		function proceed() {
			okBtn.removeEventListener('click', proceed);
			window.location.href = redirectUrl;
		}

		okBtn.addEventListener('click', proceed, { once: true });
		modal.show();
	}

	$(function () {

		app.on("app.error", function (e, data) {
			console.log(e, data);
			app.showToast(data.message, 'warning');
		});

		app.on("app.info", function (e, data) {
			console.log(e, data);
			app.showToast(data.message, 'info');
		});

		app.on("app.signout", function (e, data) {
			console.log(e, data);
			handleForcedSignout(data);
		});

		// Start SignalR hub
		app._startHub(function (connId) {
			connectionId = connId;
		});

		// Execute all queued ready callbacks
		if (app._flushReadyQueue) {
			app._flushReadyQueue();
		}
	});

})(window.app, window.jQuery);
