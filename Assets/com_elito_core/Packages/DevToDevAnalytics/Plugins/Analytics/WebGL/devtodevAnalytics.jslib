var DevToDev = {
  dtd_migrateByUser: function (appKey, migrationData) {
    try {
      window.devtodev.migrationUser(
        UTF8ToString(appKey),
        UTF8ToString(migrationData)
      );
    } catch (e) {
      _logger.error("In the migration method error has occurred: " + e);
      _logger.error("Execution of the migration method was canceled!");
    }
  },
  dtd_migrateByDevice: function (appKey, migrationData) {
    try {
      window.devtodev.migrationDevice(
        UTF8ToString(appKey),
        UTF8ToString(migrationData)
      );
    } catch (e) {
      _logger.error("In the migration method error has occurred: " + e);
      _logger.error("Execution of the migration method was canceled!");
    }
  },
  dtd_initialize: function (appKey) {
    try {
      window.devtodev.initialize(UTF8ToString(appKey), {});
    } catch (e) {
      _logger.error("In the initialize method error has occurred: " + e);
      _logger.error("Execution of the initialize method was canceled!");
    }
  },
  dtd_initializeWithConfig: function (
    appKey,
    userId,
    currentLevel,
    trackingAvailability,
    logLevel,
    applicationVersion
  ) {
    try {
      var args = {};
      args["userId"] = UTF8ToString(userId);
      args["logLevel"] = UTF8ToString(logLevel);
      args["applicationVersion"] = UTF8ToString(applicationVersion);
      if (UTF8ToString(currentLevel) != "null") {
        args["currentLevel"] = parseInt(UTF8ToString(currentLevel));
      }
      if (UTF8ToString(trackingAvailability) != "Unknown") {
        if (UTF8ToString(trackingAvailability) == "Enable") {
          args["trackingAvailability"] = true;
        } else {
          args["trackingAvailability"] = false;
        }
      }

      window.devtodev.initialize(UTF8ToString(appKey), args);
    } catch (e) {
      _logger.error("In the initialize method error has occurred: " + e);
      _logger.error("Execution of the initialize method was canceled!");
    }
  },
  dtd_initializeWithAbTestWithConfig: function (
    appKey,
    userId,
    currentLevel,
    trackingAvailability,
    logLevel,
    applicationVersion
  ) {
    try {
      var args = {};
      args["userId"] = UTF8ToString(userId);
      args["logLevel"] = UTF8ToString(logLevel);
      args["applicationVersion"] = UTF8ToString(applicationVersion);
      if (UTF8ToString(currentLevel) != "null") {
        args["currentLevel"] = parseInt(UTF8ToString(currentLevel));
      }
      if (UTF8ToString(trackingAvailability) != "Unknown") {
        if (UTF8ToString(trackingAvailability) == "Enable") {
          args["trackingAvailability"] = true;
        } else {
          args["trackingAvailability"] = false;
        }
      }
      var callbacks = {
        dtd_onReceived: function (result) {
          try {
            sendMessage("OnReceivedCallback", result.toString());
          } catch (error) {
            console.log("onReceived error: " + error);
          }
        },
        dtd_onPrepareToChange: function () {
          try {
            sendMessage("OnPrepareToChangeCallback");
          } catch (error) {
            console.log("onPrepareToChange error: " + error);
          }
        },
        dtd_onChanged: function (result, error) {
          try {
            var onChangedResult = {};
            onChangedResult["result"] = result.toString();
            onChangedResult["error"] = error;
            var json = JSON.stringify(onChangedResult);
            sendMessage("OnChangedCallback", json);
          } catch (e) {
            console.log("onChanged error: " + e);
          }
        },
      };
      window.devtodev.initializeWithAbTest(
        UTF8ToString(appKey),
        args,
        callbacks
      );
    } catch (e) {
      _logger.error("In the initialize method error has occurred: " + e);
      _logger.error("Execution of the initialize method was canceled!");
    }
  },
  dtd_initializeWithAbTest: function (appKey) {
    try {
      var callbacks = {
        dtd_onReceived: function (result) {
          try {
            sendMessage("OnReceivedCallback", result.toString());
          } catch (error) {
            console.log("onReceived error: " + error);
          }
        },
        dtd_onPrepareToChange: function () {
          try {
            sendMessage("OnPrepareToChangeCallback");
          } catch (error) {
            console.log("onPrepareToChange error: " + error);
          }
        },
        dtd_onChanged: function (result, error) {
          try {
            var onChangedResult = {};
            onChangedResult["result"] = result.toString();
            onChangedResult["error"] = error;
            var json = JSON.stringify(onChangedResult);
            sendMessage("OnChangedCallback", json);
          } catch (e) {
            console.log("onChanged error: " + e);
          }
        },
      };
      window.devtodev.initializeWithAbTest(UTF8ToString(appKey), {}, callbacks);
    } catch (e) {
      _logger.error("In the initialize method error has occurred: " + e);
      _logger.error("Execution of the initialize method was canceled!");
    }
  },
  dtd_adImpression: function (network, revenue, placement, unit) {
    try {
      window.devtodev.adImpression(
        UTF8ToString(network),
        revenue,
        UTF8ToString(placement),
        UTF8ToString(unit)
      );
    } catch (e) {
      _logger.error("In the adImpression method error has occurred: " + e);
      _logger.error("Execution of the adImpression method was canceled!");
    }
  },
  dtd_setSdkCodeVersion: function (version) {
    try {
      window.devtodev.setSdkCodeVersion(version);
    } catch (e) {
      _logger.error("In the setSdkCodeVersion method error has occurred: " + e);
      _logger.error("Execution of the setSdkCodeVersion method was canceled!");
    }
  },
  dtd_setSdkVersion: function (version) {
    try {
      window.devtodev.setSDKVersion(UTF8ToString(version));
    } catch (e) {
      _logger.error("In the setSdkVersion method error has occurred: " + e);
      _logger.error("Execution of the setSdkVersion method was canceled!");
    }
  },
  dtd_getSdkVersion: function () {
    try {
      var result = window.devtodev.getSDKVersion();
      if (typeof result === "undefined") return null;
      var buffer = _malloc(lengthBytesUTF8(result) + 1);
      var bufferSize = lengthBytesUTF8(result) + 1;
      stringToUTF8(result, buffer, bufferSize);
      return buffer;
    } catch (e) {
      _logger.error("In the getSdkVersion method error has occurred: " + e);
      _logger.error("Execution of the getSdkVersion method was canceled!");
      return null;
    }
  },
  dtd_getAppVersion: function () {
    try {
      var result = window.devtodev.getAppVersion();
      if (typeof result === "undefined") return null;
      var buffer = _malloc(lengthBytesUTF8(result) + 1);
      var bufferSize = lengthBytesUTF8(result) + 1;
      stringToUTF8(result, buffer, bufferSize);
      return buffer;
    } catch (e) {
      _logger.error("In the getAppVersion method error has occurred: " + e);
      _logger.error("Execution of the getAppVersion method was canceled!");
      return null;
    }
  },
  dtd_setAppVersion: function (version) {
    try {
      window.devtodev.setAppVersion(UTF8ToString(version));
    } catch (e) {
      _logger.error("In the setAppVersion method error has occurred: " + e);
      _logger.error("Execution of the setAppVersion method was canceled!");
    }
  },
  dtd_getUserId: function () {
    try {
      var result = window.devtodev.getUserId();
      if (typeof result === "undefined") return null;
      var buffer = _malloc(lengthBytesUTF8(result) + 1);
      var bufferSize = lengthBytesUTF8(result) + 1;
      stringToUTF8(result, buffer, bufferSize);
      return buffer;
    } catch (e) {
      _logger.error("In the getUserId method error has occurred: " + e);
      _logger.error("Execution of the getUserId method was canceled!");
      return null;
    }
  },
  dtd_setUserId: function (userId) {
    try {
      window.devtodev.setUserId(UTF8ToString(userId));
    } catch (e) {
      _logger.error("In the setUserId method error has occurred: " + e);
      _logger.error("Execution of the setUserId method was canceled!");
    }
  },
  dtd_getCurrentLevel: function () {
    try {
      var result = window.devtodev.getCurrentLevel();
      if (typeof result === "undefined") return null;
      var buffer = _malloc(lengthBytesUTF8(result) + 1);
      var bufferSize = lengthBytesUTF8(result) + 1;
      stringToUTF8(result, buffer, bufferSize);
      return buffer;
    } catch (e) {
      _logger.error("In the getCurrentLevel method error has occurred: " + e);
      _logger.error("Execution of the getCurrentLevel method was canceled!");
      return 0;
    }
  },
  dtd_setCurrentLevel: function (level) {
    try {
      window.devtodev.setCurrentLevel(level);
    } catch (e) {
      _logger.error("In the setCurrentLevel method error has occurred: " + e);
      _logger.error("Execution of the setCurrentLevel method was canceled!");
    }
  },
  dtd_getLogLevel: function () {
    try {
      var result = window.devtodev.logLevel;
      if (typeof result === "undefined") return null;
      var buffer = _malloc(lengthBytesUTF8(result) + 1);
      var bufferSize = lengthBytesUTF8(result) + 1;
      stringToUTF8(result, buffer, bufferSize);
      return buffer;
    } catch (e) {
      _logger.error("In the getLogLevel method error has occurred: " + e);
      _logger.error("Execution of the getLogLevel method was canceled!");
      return null;
    }
  },
  dtd_setLogLevel: function (logLevel) {
    try {
      window.devtodev.logLevel = UTF8ToString(logLevel);
    } catch (e) {
      _logger.error("In the setLogLevel method error has occurred: " + e);
      _logger.error("Execution of the setLogLevel method was canceled!");
    }
  },
  dtd_getTrackingAvailability: function () {
    try {
      return window.devtodev.getTrackingAvailability();
    } catch (e) {
      _logger.error(
        "In the getTrackingAvailability method error has occurred: " + e
      );
      _logger.error(
        "Execution of the getTrackingAvailability method was canceled!"
      );
      return true;
    }
  },
  dtd_setTrackingAvailability: function (value) {
    try {
      window.devtodev.setTrackingAvailability(!!value);
    } catch (e) {
      _logger.error(
        "In the setTrackingAvailability method error has occurred: " + e
      );
      _logger.error(
        "Execution of the setTrackingAvailability method was canceled!"
      );
    }
  },
  dtd_referrer: function (value) {
    try {
      if (value == null || UTF8ToString(value).length == 0) {
        window.devtodev.referrer({});
      }
      var utm = JSON.parse(UTF8ToString(value));
      window.devtodev.referrer(utm);
    } catch (e) {
      _logger.error("In the referrer method error has occurred: " + e);
      _logger.error("Execution of the referrer method was canceled!");
    }
  },
  dtd_virtualCurrencyPayment: function (
    purchaseId,
    purchaseType,
    purchaseAmount,
    args
  ) {
    try {
      var purchaseId = UTF8ToString(purchaseId);
      var purchaseType = UTF8ToString(purchaseType);
      var purchaseAmount = purchaseAmount;
      var data = {};
      if (args != null || UTF8ToString(args).length != 0) {
        data = JSON.parse(UTF8ToString(args));
      }
      window.devtodev.virtualCurrencyPayment(
        purchaseId,
        purchaseType,
        purchaseAmount,
        data
      );
    } catch (e) {
      _logger.error(
        "In the virtualCurrencyPayment method error has occurred: " + e
      );
      _logger.error(
        "Execution of the virtualCurrencyPayment method was canceled!"
      );
    }
  },
  dtd_currencyAccrual: function (
    currencyName,
    currencyAmount,
    source,
    accrualType
  ) {
    try {
      window.devtodev.currencyAccrual(
        UTF8ToString(currencyName),
        currencyAmount,
        UTF8ToString(source),
        accrualType
      );
    } catch (e) {
      _logger.error("In the currencyAccrual method error has occurred: " + e);
      _logger.error("Execution of the currencyAccrual method was canceled!");
    }
  },
  dtd_realCurrencyPayment: function (orderId, price, productId, currencyCode) {
    try {
      window.devtodev.realCurrencyPayment(
        UTF8ToString(orderId),
        price,
        UTF8ToString(productId),
        UTF8ToString(currencyCode)
      );
    } catch (e) {
      _logger.error(
        "In the realCurrencyPayment method error has occurred: " + e
      );
      _logger.error(
        "Execution of the realCurrencyPayment method was canceled!"
      );
    }
  },
  dtd_tutorial: function (step) {
    window.devtodev.tutorial(step);
  },
  dtd_socialNetworkConnect: function (name) {
    try {
      window.devtodev.socialNetworkConnect(UTF8ToString(name));
    } catch (e) {
      _logger.error(
        "In the socialNetworkConnect method error has occurred: " + e
      );
      _logger.error(
        "Execution of the socialNetworkConnect method was canceled!"
      );
    }
  },
  dtd_socialNetworkPost: function (name, reason) {
    try {
      window.devtodev.socialNetworkPost(
        UTF8ToString(name),
        UTF8ToString(reason)
      );
    } catch (e) {
      _logger.error("In the socialNetworkPost method error has occurred: " + e);
      _logger.error("Execution of the socialNetworkPost method was canceled!");
    }
  },
  dtd_sendBufferedEvents: function () {
    window.devtodev.sendBufferedEvents();
  },
  dtd_startProgressionEvent: function (val, valJSON) {
    try {
      if (valJSON == null || UTF8ToString(valJSON).length == 0) {
        window.devtodev.startProgressionEvent(UTF8ToString(val), {});
        return;
      }

      var json = JSON.parse(UTF8ToString(valJSON));
      var validatedObject = {};
      if (json["source"] != null && json["source"].length > 0)
        validatedObject["source"] = json["source"];
      if (json["difficulty"] != null)
        validatedObject["difficulty"] = json["difficulty"];
      window.devtodev.startProgressionEvent(UTF8ToString(val), validatedObject);
    } catch (e) {
      _logger.error(
        "In the startProgressionEvent method error has occurred: " + e
      );
      _logger.error(
        "Execution of the startProgressionEvent method was canceled!"
      );
    }
  },
  dtd_finishProgressionEvent: function (val, valJSON) {
    try {
      if (valJSON == null || UTF8ToString(valJSON).length == 0) {
        window.devtodev.finishProgressionEvent(UTF8ToString(val), {});
        return;
      }

      var json = JSON.parse(UTF8ToString(valJSON));
      console.log(UTF8ToString(valJSON));
      var validatedObject = {};
      validatedObject["successfulCompletion"] = json["successfulCompletion"];
      if (json["duration"] != null && json["duration"] > 0)
        validatedObject["duration"] = json["duration"];
      if (json["spent"] != null && Object.keys(json["spent"]).length > 0)
        validatedObject["spent"] = json["spent"];
      if (json["earned"] != null && Object.keys(json["earned"]).length > 0)
        validatedObject["earned"] = json["earned"];
      window.devtodev.finishProgressionEvent(
        UTF8ToString(val),
        validatedObject
      );
    } catch (e) {
      _logger.error(
        "In the finishProgressionEvent method error has occurred: " + e
      );
      _logger.error(
        "Execution of the finishProgressionEvent method was canceled!"
      );
    }
  },
  dtd_customEvent: function (name, params) {
    try {
      if (params == null || UTF8ToString(params).length == 0) {
        window.devtodev.customEvent(UTF8ToString(name));
        return;
      }
      var customParams = JSON.parse(UTF8ToString(params));
      window.devtodev.customEvent(UTF8ToString(name), customParams);
    } catch (e) {
      _logger.error(
        "In the customEvent method error has occurred: for key " +
          UTF8ToString(name) +
          ":" +
          e
      );
      _logger.error("Execution of the customEvent method was canceled!");
    }
  },
  dtd_replaceUserId: function (previousUserId, userId) {
    try {
      window.devtodev.replaceUserId(
        UTF8ToString(previousUserId),
        UTF8ToString(userId)
      );
    } catch (e) {
      _logger.error("In the replaceUserId method error has occurred: " + e);
      _logger.error("Execution of the replaceUserId method was canceled!");
    }
  },
  dtd_levelUp: function (level) {
    try {
      window.devtodev.levelUp(level);
    } catch (e) {
      _logger.error("In the levelUp method error has occurred: " + e);
      _logger.error("Execution of the levelUp method was canceled!");
    }
  },
  dtd_levelUpWithResources: function (
    level,
    jsonBalance,
    jsonSpent,
    jsonEarned,
    jsonBought
  ) {
    try {
      var balance = JSON.parse(UTF8ToString(jsonBalance));
      var spent = JSON.parse(UTF8ToString(jsonSpent));
      var earned = JSON.parse(UTF8ToString(jsonEarned));
      var bought = JSON.parse(UTF8ToString(jsonBought));
      window.devtodev.levelUp(level, balance, spent, earned, bought);
    } catch (e) {
      _logger.error("In the levelUp method error has occurred: " + e);
      _logger.error("Execution of the levelUp method was canceled!");
    }
  },
  dtd_currentBalance: function (jsonString) {
    try {
      if (jsonString == null || UTF8ToString(jsonString).length == 0) {
        window.devtodev.currentBalance(null);
        return;
      }
      var convertedJson = UTF8ToString(jsonString);
      var balance = JSON.parse(convertedJson);
      window.devtodev.currentBalance(balance);
    } catch (e) {
      _logger.error("In the currentBalance method error has occurred: " + e);
      _logger.error("Execution of the currentBalance method was canceled!");
    }
  },
  dtd_setTestProxyUrl: function (url) {
    window.devtodev.testProxyUrl = UTF8ToString(url);
  },
  dtd_setTestCustomUrl: function (url) {
    window.devtodev.testCustomUrl = UTF8ToString(url);
  },
  dtd_getTestProxyUrl: function (url) {
    var result = window.devtodev.testProxyUrl;
    if (typeof result === "undefined") return null;
    var buffer = _malloc(lengthBytesUTF8(result) + 1);
    var bufferSize = lengthBytesUTF8(result) + 1;
    stringToUTF8(result, buffer, bufferSize);
    return buffer;
  },
  dtd_getTestCustomUrl: function (url) {
    var result = window.devtodev.testCustomUrl;
    if (typeof result === "undefined") return null;
    var buffer = _malloc(lengthBytesUTF8(result) + 1);
    var bufferSize = lengthBytesUTF8(result) + 1;
    stringToUTF8(result, buffer, bufferSize);
    return buffer;
  },
  dtd_testLogs: function () {
    window.devtodev.testLogs();
  },
};
mergeInto(LibraryManager.library, DevToDev);
