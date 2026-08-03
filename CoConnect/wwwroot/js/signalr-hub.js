(function (app, $) {

    // Event hub that persists across reconnections
    var eventHub = $({});
    var trackedEvents = [];

    // Current SignalR connection (null when disconnected)
    var hubConnection = null;

    // Bind an event to the current SignalR connection
    function bindEventToConnection(eventName) {
        if (hubConnection) {
            console.log("Binding event '" + eventName + "' to hub connection.");
            hubConnection.on(eventName, function (data) {
                eventHub.trigger(eventName, data);
            });
        }
    }

    // Public API: Register event handler
    app.on = function (eventName, handler) {
        // Track this event if it's new
        if (!trackedEvents.includes(eventName)) {
            trackedEvents.push(eventName);
            bindEventToConnection(eventName); // Bind now if connected
        }

        // Add handler to event hub
        eventHub.on(eventName, handler);
    };

    // Internal: Start SignalR hub (called automatically by app-start.js)
    app._startHub = function (onConnected) {
        console.log("Connecting to SignalR Hub...");

        var h = new signalR.HubConnectionBuilder()
            .withUrl('/messagehub')
            .build();

        // Helper to handle reconnection
        function reconnect(reason) {
            console.log(reason + " Reconnecting in 5s...");
            hubConnection = null;
            setTimeout(function () {
                app._startHub(onConnected);
            }, 5000);
        }

        // Register onclose BEFORE starting connection
        h.onclose(function () {
            reconnect("Hub disconnected.");
        });

        h.start()
            .then(function () {
                console.log("Hub connected.");
                hubConnection = h;

                // Bind all tracked events to the new connection
                trackedEvents.forEach(bindEventToConnection);

                // Invoke callback if provided
                if (onConnected) {
                    h.invoke("GetConnectionId").then(function (connectionId) {
                        console.log("Connection ID: " + connectionId);
                        onConnected(connectionId);
                    });
                }
            })
            .catch(function (error) {
                console.error("Hub connection failed:", error);
                reconnect("Connection failed.");
            });
    };

})(window.app, window.jQuery);
