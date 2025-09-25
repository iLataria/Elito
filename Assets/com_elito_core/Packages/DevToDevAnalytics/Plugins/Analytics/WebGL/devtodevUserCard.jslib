var UserCard = {
  dtd_userCheater: function (value) {
    window.devtodev.user.setCheater(!!value);
  },
  dtd_userTester: function (value) {
    window.devtodev.user.setTester(!!value);
  },
  dtd_userUnset: function (jsonKeys) {
    if (jsonKeys == null || UTF8ToString(jsonKeys).length == 0) {
      window.devtodev.user.unset(null);
      return;
    }

    var keys = JSON.parse(UTF8ToString(jsonKeys));
    window.devtodev.user.unset(keys);
  },
  dtd_userDelete: function () {
    window.devtodev.user.clearUser();
  },
  dtd_userSet: function (key, jsonValue) {
    if (jsonValue == null || UTF8ToString(jsonValue).length == 0) {
      window.devtodev.user.set(UTF8ToString(key), null);
      return;
    }
    var data = JSON.parse(UTF8ToString(jsonValue));
    window.devtodev.user.set(UTF8ToString(key), data);
  },
  dtd_userSetOnce: function (json) {
    var data = JSON.parse(UTF8ToString(json));
    var key = data.key;
    var value = data.value;
    window.devtodev.user.setOnce(key, value);
  },
  dtd_userGet: function (key) {
    var value = window.devtodev.user.getValue(UTF8ToString(key));
    if (value == null) return null;
    var json = JSON.stringify(value);
    var buffer = _malloc(lengthBytesUTF8(json) + 1);
    var bufferSize = lengthBytesUTF8(json) + 1;
    stringToUTF8(json, buffer, bufferSize);
    return buffer;
  },
};

mergeInto(LibraryManager.library, UserCard);
