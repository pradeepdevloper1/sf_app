"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.bytesToSize = void 0;
function bytesToSize(bytes) {
    var sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
    if (bytes === 0)
        return 'n/a';
    var i = Math.floor(Math.log(bytes) / Math.log(1024));
    if (i === 0)
        return bytes + " " + sizes[i] + ")";
    return (bytes / (Math.pow(1024, i))).toFixed(1) + " " + sizes[i];
}
exports.bytesToSize = bytesToSize;
//# sourceMappingURL=helper.js.map