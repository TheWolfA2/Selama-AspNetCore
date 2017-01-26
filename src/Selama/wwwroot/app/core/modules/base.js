"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var core_1 = require("@angular/core");
var material_1 = require("@angular/material");
var platform_browser_1 = require("@angular/platform-browser");
var ng2_slim_loading_bar_1 = require("ng2-slim-loading-bar");
var progress_bar_1 = require("../services/progress-bar");
var BaseModule = (function () {
    function BaseModule() {
    }
    return BaseModule;
}());
BaseModule = __decorate([
    core_1.NgModule({
        imports: [
            platform_browser_1.BrowserModule,
            material_1.MaterialModule.forRoot(),
            ng2_slim_loading_bar_1.SlimLoadingBarModule.forRoot(),
        ],
        providers: [
            progress_bar_1.ProgressBarService,
        ],
        exports: [
            platform_browser_1.BrowserModule,
            material_1.MaterialModule,
            ng2_slim_loading_bar_1.SlimLoadingBarModule,
        ]
    })
], BaseModule);
exports.BaseModule = BaseModule;
//# sourceMappingURL=base.js.map