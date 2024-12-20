# Change Log

All notable changes to this project will be documented in this file. See [versionize](https://github.com/versionize/versionize) for commit guidelines.

<a name="4.1.1"></a>
## [4.1.1](https://www.github.com/mu88/ThisIsYourLife/releases/tag/v4.1.1) (2024-12-20)

<a name="4.1.0"></a>
## [4.1.0](https://www.github.com/mu88/ThisIsYourLife/releases/tag/v4.1.0) (2024-12-20)

### Features

* add health check ([36bcf17](https://www.github.com/mu88/ThisIsYourLife/commit/36bcf1797b27bbc23901e813dddaf63240518916))
* add reusable workflow ([fafb6b1](https://www.github.com/mu88/ThisIsYourLife/commit/fafb6b1de46c1940c4e8c6a8698881ac615c2f13))
* embed health check tool ([5ae26ec](https://www.github.com/mu88/ThisIsYourLife/commit/5ae26ece4da8b0a190438f3d6d4620d8d7f43808))

<a name="4.0.0"></a>
## [4.0.0](https://www.github.com/mu88/ThisIsYourLife/releases/tag/v4.0.0) (2024-12-06)

### Features

* **deps:** upgrade to .NET 9 ([30a5991](https://www.github.com/mu88/ThisIsYourLife/commit/30a59917e0d19261fe45de41c890d2894290c232))

### Breaking Changes

* **deps:** upgrade to .NET 9 ([30a5991](https://www.github.com/mu88/ThisIsYourLife/commit/30a59917e0d19261fe45de41c890d2894290c232))

<a name="3.2.0"></a>
## [3.2.0](https://www.github.com/mu88/ThisIsYourLife/releases/tag/v3.2.0) (2024-08-03)

### Features

* replace OpenTelemetry and multi-manifest Docker image logic with NuGet package mu88.Shared ([e355df5](https://www.github.com/mu88/ThisIsYourLife/commit/e355df5bbcf4e885cbd946113401bba47a2078a2))

<a name="3.1.0"></a>
## [3.1.0](https://www.github.com/mu88/ThisIsYourLife/releases/tag/v3.1.0) (2024-07-05)

### Features

* add OpenTelemetry ([fd64a8c](https://www.github.com/mu88/ThisIsYourLife/commit/fd64a8c60d83efdd9100a7d19e45b51e6c0dda34))
* filter LifePoints by year or creator ([2e5056d](https://www.github.com/mu88/ThisIsYourLife/commit/2e5056dda6f54f73f02cc37bccc5fb43af315cd4))
* increase maximum file size of images to 20 MB ([d6a0114](https://www.github.com/mu88/ThisIsYourLife/commit/d6a01149c5fc6a46a0c84bdbc07bce67a45ef117))
* make deletion configurable ([3ac7177](https://www.github.com/mu88/ThisIsYourLife/commit/3ac7177f5f633e5ff5a84c4d624a8b6ca37ddbf6))
* show loading indicator when loading and filtering lifepoints ([d373976](https://www.github.com/mu88/ThisIsYourLife/commit/d3739762bab3874db7cfe143a31a2a748192166f))
* use .NET 8 ([2fe0c2f](https://www.github.com/mu88/ThisIsYourLife/commit/2fe0c2fe18be39a569e027ca40c7b398ec82792d))

### Bug Fixes

* don't rename assembly as it breaks the frontend ([8455402](https://www.github.com/mu88/ThisIsYourLife/commit/8455402e7511f1e621a1540fcf684ad9fe8412e4))
* ensure that app doesn't fail when upload is no image ([ddabad4](https://www.github.com/mu88/ThisIsYourLife/commit/ddabad4f032a220915ac5386d056d910821cefee))
* fix path ([e3bc187](https://www.github.com/mu88/ThisIsYourLife/commit/e3bc187d92d93761318269aa3e587356fd4b6e6c))
* prevent app crash if Leaflet map is null ([2c4825c](https://www.github.com/mu88/ThisIsYourLife/commit/2c4825c32805bf9c85db7111bbcce5099c94c335))
* remove BOM from startup script ([d7d0bbe](https://www.github.com/mu88/ThisIsYourLife/commit/d7d0bbef1515bc205293a293af958b83993918a4))
* remove temporal dependency from test ([04c682c](https://www.github.com/mu88/ThisIsYourLife/commit/04c682c6169bfecce88b36a6f074ea8276c1e837))
* resize and pan popup automatically so that it remains visible ([d2b7492](https://www.github.com/mu88/ThisIsYourLife/commit/d2b74922dff90b107f81b44ed7f650ff29a0d343))
* use relative URL path for Leaflet components ([60fbd30](https://www.github.com/mu88/ThisIsYourLife/commit/60fbd30952b6e2b4174f3b3623ece2bf5847950c))

### Breaking Changes

* use .NET 8 ([2fe0c2f](https://www.github.com/mu88/ThisIsYourLife/commit/2fe0c2fe18be39a569e027ca40c7b398ec82792d))

<a name="3.1.0"></a>
## [3.1.0](https://www.github.com/mu88/ThisIsYourLife/releases/tag/v3.1.0) (2024-06-16)

### Features

* add OpenTelemetry ([b0247bc](https://www.github.com/mu88/ThisIsYourLife/commit/b0247bcab61fd0c4050429b6dd0ab61395f6a0c5))

<a name="3.0.5"></a>
## [3.0.5](https://www.github.com/mu88/ThisIsYourLife/releases/tag/v3.0.5) (2024-01-31)

<a name="3.0.4"></a>
## [3.0.4](https://www.github.com/mu88/ThisIsYourLife/releases/tag/v3.0.4) (2023-11-19)

### Bug Fixes

* fix path ([e3bc187](https://www.github.com/mu88/ThisIsYourLife/commit/e3bc187d92d93761318269aa3e587356fd4b6e6c))

<a name="3.0.3"></a>
## [3.0.3](https://www.github.com/mu88/ThisIsYourLife/releases/tag/v3.0.3) (2023-11-19)

<a name="3.0.2"></a>
## [3.0.2](https://www.github.com/mu88/ThisIsYourLife/releases/tag/v3.0.2) (2023-11-17)

### Bug Fixes

* don't rename assembly as it breaks the frontend ([8455402](https://www.github.com/mu88/ThisIsYourLife/commit/8455402e7511f1e621a1540fcf684ad9fe8412e4))

<a name="3.0.1"></a>
## [3.0.1](https://www.github.com/mu88/ThisIsYourLife/releases/tag/v3.0.1) (2023-11-17)

<a name="3.0.0"></a>
## [3.0.0](https://www.github.com/mu88/ThisIsYourLife/releases/tag/v3.0.0) (2023-11-17)

### Features

* use .NET 8 ([2fe0c2f](https://www.github.com/mu88/ThisIsYourLife/commit/2fe0c2fe18be39a569e027ca40c7b398ec82792d))

### Breaking Changes

* use .NET 8 ([2fe0c2f](https://www.github.com/mu88/ThisIsYourLife/commit/2fe0c2fe18be39a569e027ca40c7b398ec82792d))

<a name="2.2.0"></a>
## [2.2.0](https://www.github.com/mu88/ThisIsYourLife/releases/tag/v2.2.0) (2023-11-17)

### Features

* filter LifePoints by year or creator ([2e5056d](https://www.github.com/mu88/ThisIsYourLife/commit/2e5056dda6f54f73f02cc37bccc5fb43af315cd4))
* increase maximum file size of images to 20 MB ([d6a0114](https://www.github.com/mu88/ThisIsYourLife/commit/d6a01149c5fc6a46a0c84bdbc07bce67a45ef117))
* make deletion configurable ([3ac7177](https://www.github.com/mu88/ThisIsYourLife/commit/3ac7177f5f633e5ff5a84c4d624a8b6ca37ddbf6))
* show loading indicator when loading and filtering lifepoints ([d373976](https://www.github.com/mu88/ThisIsYourLife/commit/d3739762bab3874db7cfe143a31a2a748192166f))

### Bug Fixes

* ensure that app doesn't fail when upload is no image ([ddabad4](https://www.github.com/mu88/ThisIsYourLife/commit/ddabad4f032a220915ac5386d056d910821cefee))
* prevent app crash if Leaflet map is null ([2c4825c](https://www.github.com/mu88/ThisIsYourLife/commit/2c4825c32805bf9c85db7111bbcce5099c94c335))
* remove BOM from startup script ([d7d0bbe](https://www.github.com/mu88/ThisIsYourLife/commit/d7d0bbef1515bc205293a293af958b83993918a4))
* remove temporal dependency from test ([04c682c](https://www.github.com/mu88/ThisIsYourLife/commit/04c682c6169bfecce88b36a6f074ea8276c1e837))
* resize and pan popup automatically so that it remains visible ([d2b7492](https://www.github.com/mu88/ThisIsYourLife/commit/d2b74922dff90b107f81b44ed7f650ff29a0d343))
* use relative URL path for Leaflet components ([60fbd30](https://www.github.com/mu88/ThisIsYourLife/commit/60fbd30952b6e2b4174f3b3623ece2bf5847950c))

