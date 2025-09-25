var devtodevPlugin = {
    /**
     * Get IndexedDB availability
     * @return bool
     */
    dtd_isIndexedDBAvailable: function() {
        var isAvailable = false;
        window.indexedDB = window.indexedDB ||
            window.mozIndexedDB ||
            window.webkitIndexedDB ||
            window.msIndexedDB;

        if (window.indexedDB) isAvailable = true;

        return isAvailable;
    },

    /**
     * Get storage availability
     * @return bool
     */
    dtd_isStorageAvailable: function() {
        try {
            const key = "__example_key__";
            window.localStorage.setItem(key, key);
            window.localStorage.removeItem(key);
            return true;
        } catch (e) {
            return false;
        }
    },

    /**
     * @param key string
     * @return void
     */
    dtd_removeItem: function(key) {
        try {
            window.localStorage.removeItem(UTF8ToString(key));
        } catch (e) {}

    },

    /**
     * @param key string
     * @param value string
     * @return void
     */
    dtd_setItem: function(key, value) {
        try {
            window.localStorage.setItem(UTF8ToString(key), UTF8ToString(value));
        } catch (e) {}
    },

    /**
     * @param key string
     * @return object || null
     */
    dtd_getItem: function(key) {
        try {
            var result = window.localStorage.getItem(UTF8ToString(key));
            result = result === 'undefined' ? null : result;
            if (result != null) {
                var buffer = _malloc(lengthBytesUTF8(result) + 1);
                writeStringToMemory(result, buffer);
                return buffer;
            }
        } catch (e) {}

        return null;
    },

    /**
     * @param key string
     * @return bool
     */
    dtd_isExistItem: function(key) {
        try {
            var result = window.localStorage.getItem(key);
            result = result === 'undefined' ? false : true;
            return result;
        } catch (e) {}

        return false;
    }
};
mergeInto(LibraryManager.library, devtodevPlugin);