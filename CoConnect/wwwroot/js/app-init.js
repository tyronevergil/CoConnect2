(function (app) {

    var readyQueue = [];
    var isReady = false;

    // Queue or execute functions based on ready state
    app.ready = function (args, callback) {
        // Support both: app.ready(callback) and app.ready(args, callback)
        if (typeof args === 'function') {
            callback = args;
            args = undefined;
        }

        if (typeof callback !== 'function') {
            console.warn('app.ready requires a function');
            return;
        }

        if (isReady) {
            // DOM is ready, execute immediately with window context
            callback.apply(window, args);
        } else {
            // DOM not ready yet, queue for later
            readyQueue.push({ args: args, callback: callback });
        }
    };

    // Internal: Called by app-start.js when DOM is ready
    app._flushReadyQueue = function () {
        isReady = true;

        // Execute all queued callbacks with window context
        while (readyQueue.length > 0) {
            var item = readyQueue.shift();
            item.callback.apply(window, item.args);
        }
    };

})(window.app = window.app || {});
