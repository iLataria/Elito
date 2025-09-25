mergeInto(LibraryManager.library, {
  $instance: null,
  $getInstance: function () {
    const instance =
      typeof gameInstance !== "undefined"
        ? gameInstance
        : typeof unityInstance !== "undefined"
        ? unityInstance
        : typeof myGameInstance !== "undefined"
        ? myGameInstance
        : null;
    return instance;
  },
  $dtd_sendMessage: function (method, message) {
    const instance = getInstance();
    if (instance && instance.SendMessage) {
      try {
        instance.SendMessage(
          "[devtodev_AsyncOperationDispatcher]",
          method,
          message
        );
      } catch (e) {
        console.log("[DevToDev] Unity SendMessage Error:", e);
      }
    } else if (typeof SendMessage !== "undefined") {
      try {
        SendMessage("[devtodev_AsyncOperationDispatcher]", method, message);
      } catch (e) {
        console.log("[DevToDev] SendMessage Error:", e);
      }
    } else {
      console.log(
        "[DevToDev] Error: Unable to find Unity instance. Modify your WebGL template to create unityInstance variable."
      );
    }
  },
  $identifierCallback: function (id) {
    dtd_sendMessage("OnIdentifierReceived", id.toString());
  },
  $loggerCallback: function (logLevel, message) {
    const logMessage = logLevel + "|" + logLevel.toUpperCase() + " " + message;
    dtd_sendMessage("OnLogReceived", logMessage);
  },
  dtd_setLoggerCallback: function () {
    window.devtodev.onDebugMessage(loggerCallback);
  },
  dtd_setIdentifierCallback: function () {
    window.devtodev.setIdentifiersListener(identifierCallback);
  },
  $initializationCompleteCallback: function () {
    dtd_sendMessage("OnInitializationCompleteReceived");
  },
  dtd_setInitializationCompleteCallback: function () {
    window.devtodev.setInitializationCompleteCallback(
      initializationCompleteCallback
    );
  },
  dtd_setIdentifierCallback__deps: [
    "$instance",
    "$getInstance",
    "$identifierCallback",
    "$dtd_sendMessage",
  ],
  dtd_setInitializationCompleteCallback__deps: [
    "$instance",
    "$getInstance",
    "$initializationCompleteCallback",
    "$dtd_sendMessage",
  ],
  dtd_setLoggerCallback__deps: [
    "$instance",
    "$getInstance",
    "$loggerCallback",
    "$dtd_sendMessage",
  ],
});
