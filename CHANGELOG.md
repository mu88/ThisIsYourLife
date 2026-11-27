# Change Log

All notable changes to this project will be documented in this file. See [versionize](https://github.com/versionize/versionize) for commit guidelines.

<a name="7.0.0"></a>
## [7.0.0](https://www.github.com/mu88/ThisIsYourLife/releases/tag/7.0.0) (2026-11-27)

### ✨ Features

* implement storage options configuration and refactor related services ([2b6b692](https://www.github.com/mu88/ThisIsYourLife/commit/2b6b69259d50b7c41ebb8e2f9a7997c726f425d7))
* migrate code coverage from coverlet to dotnet-coverage ([dfb51a7](https://www.github.com/mu88/ThisIsYourLife/commit/dfb51a785d50ff8aaffef03512a95d09de21cfe4))
* migrate to .NET 11 ([473965a](https://www.github.com/mu88/ThisIsYourLife/commit/473965a2487521ea6904d249df779fc741a9f171))
* restructure error handling and routing with new Razor components ([9416be4](https://www.github.com/mu88/ThisIsYourLife/commit/9416be4def1739c6e3b4cd2b85264b3ab5f658e3))

### 🐛 Bug Fixes

* remove dead MainLayout.razor.css ([d2913fa](https://www.github.com/mu88/ThisIsYourLife/commit/d2913fab4b9fad52e548005ab3d43c7b375eb20d))
* resolve Sonar code smells and close ImageService coverage gap ([8191a90](https://www.github.com/mu88/ThisIsYourLife/commit/8191a90ccba3c44831e6dbe68f7582212076f981))
* update vulnerable .NET runtime ([f51b655](https://www.github.com/mu88/ThisIsYourLife/commit/f51b6551335532ce4299af6b0eaf13031921a77f))
* **ci:** restore mutation tool in its execution directory ([2902ed6](https://www.github.com/mu88/ThisIsYourLife/commit/2902ed609595097f82aa78f1b4f47870f1e7beb9))
* **ci:** use one SDK for mutation testing ([a484b39](https://www.github.com/mu88/ThisIsYourLife/commit/a484b3982129e94c145adc0029d4455b46f1133a))
* **container:** bump base image to 10.0.13 to patch runtime CVEs ([87fce88](https://www.github.com/mu88/ThisIsYourLife/commit/87fce88177d523c8375b3a6ffc63f58658dfc482))
* **deps:** adopt shared testing publish worker fix ([ebcb318](https://www.github.com/mu88/ThisIsYourLife/commit/ebcb318248af5e03474637766f6acf61118afb6d))
* **deps:** override vulnerable SQLite transitive dependency ([528a03c](https://www.github.com/mu88/ThisIsYourLife/commit/528a03cfbc527786dd4f5ff2f2a157a22fafaf3d))
* **security:** patch WebUtilities dependency ([144d8ba](https://www.github.com/mu88/ThisIsYourLife/commit/144d8ba5be986a8bb79987d9a26cc1543ec5d9e1))
* **tests:** use valid SemVer for system test container image tag ([7f57ba8](https://www.github.com/mu88/ThisIsYourLife/commit/7f57ba8f90ab2b6534432bf3f2f618e81ce8e612))

### ♻️ Refactors

* code review improvements and configurable map ([1421b7d](https://www.github.com/mu88/ThisIsYourLife/commit/1421b7d98f667431ce5d5aa5973aa959417f67b8))
* implement tracing for method execution and refactor logging approach ([b0ed429](https://www.github.com/mu88/ThisIsYourLife/commit/b0ed42993e160bfbc0f4df449da4819851175a36))
* remove Serilog dependencies and update logging configuration ([bd85838](https://www.github.com/mu88/ThisIsYourLife/commit/bd858380f2050ab6c6b84596fb9f610c5366b246))
* replace AutoMapper with explicit mapping extensions ([8a0b35e](https://www.github.com/mu88/ThisIsYourLife/commit/8a0b35e3390d71c3d7a3b98d37d6715567881b49))
* switch from ImageSharp to SkiaSharp due to licensing ([e4a7c20](https://www.github.com/mu88/ThisIsYourLife/commit/e4a7c2040be3666e142d9ed2546df38faae57ba7))
* **ci:** use centralized pin guard ([d14c5ee](https://www.github.com/mu88/ThisIsYourLife/commit/d14c5ee36f4f77fc3bd306c7e62b6961c28bbf32))
* **deps:** remove obsolete sqlite overrides ([1b2aaa6](https://www.github.com/mu88/ThisIsYourLife/commit/1b2aaa686ea73dbcf0a2652c988b146ffc956dcc))
* **tests:** adopt mu88.Shared.Testing for System test infrastructure ([fbcd394](https://www.github.com/mu88/ThisIsYourLife/commit/fbcd394b12a0c6dcefa6a058a50c19bb6305fc6d))

### ✅ Tests

* improve system test ([debd9da](https://www.github.com/mu88/ThisIsYourLife/commit/debd9da576b176b51d5aedb7b0a2f4b4918cd1b4))
* push code coverage ([7e804f5](https://www.github.com/mu88/ThisIsYourLife/commit/7e804f505a042fae932afe015dba8b79d4b38e6f))
* simplify build process by using CliWrap ([251d4fd](https://www.github.com/mu88/ThisIsYourLife/commit/251d4fd7fd83f95156d3698e90206970857a1595))

### 🔧 Chores

* apply new code style ([df41fe3](https://www.github.com/mu88/ThisIsYourLife/commit/df41fe36d5d5dda3571679d0e9ae73fb1f8c4948))
* fix Sonar issues ([c9be5b6](https://www.github.com/mu88/ThisIsYourLife/commit/c9be5b6e3d1853212cc03392fac1a398d2fcfab9))
* resolve Sonar issues and push test coverage ([b753395](https://www.github.com/mu88/ThisIsYourLife/commit/b753395a7f950150a56ff8227beae099d632618e))
* simplify launch settings by removing IIS Express configuration ([b19c7c0](https://www.github.com/mu88/ThisIsYourLife/commit/b19c7c08e7123390c622fc939d9875a5911f3792))
* use new shared GitHub repo mu88/common ([de5d740](https://www.github.com/mu88/ThisIsYourLife/commit/de5d7405ae87789790f43757f238e71abf10252a))
* **ci:** remove temporary mutation test trigger ([dbae4b2](https://www.github.com/mu88/ThisIsYourLife/commit/dbae4b23454e85bee128beefc1464a680b0df6f4))
* **deps:** bump Microsoft.Extensions.* and EF Core to 10.0.11 ([6a8dbb6](https://www.github.com/mu88/ThisIsYourLife/commit/6a8dbb6714a26690badfeb72880b23553fa03004))
* **deps:** pin dependencies ([d4772c5](https://www.github.com/mu88/ThisIsYourLife/commit/d4772c50ad0db2487639b1ece8bbcb35ada7af3f))
* **deps:** resolve security vulnerability by upgrading `AutoMapper` ([d173d2d](https://www.github.com/mu88/ThisIsYourLife/commit/d173d2d497da4feb59ef93ba7a5404b2972a41f1))
* **deps:** update all .net ([21033e4](https://www.github.com/mu88/ThisIsYourLife/commit/21033e440abe725742e6780538d2acf4518fec31))
* **deps:** update all .net ([0fc8f9d](https://www.github.com/mu88/ThisIsYourLife/commit/0fc8f9de900f3739b2bce5d49fdf90064f30513d))
* **deps:** update all .net ([7d0d4a2](https://www.github.com/mu88/ThisIsYourLife/commit/7d0d4a27e3e02bc3d0bf3754ba544991c8bfb22e))
* **deps:** update all .net ([96fa955](https://www.github.com/mu88/ThisIsYourLife/commit/96fa95525105144fa1c62538305cad00e5cacd18))
* **deps:** update all .net ([508d49f](https://www.github.com/mu88/ThisIsYourLife/commit/508d49fa8bbb81133dd207b6c348383a6220b6ed))
* **deps:** update all .net ([7cc665a](https://www.github.com/mu88/ThisIsYourLife/commit/7cc665ab487cf61770b2d86d42dc31ddc5496e0c))
* **deps:** update all dependencies ([623c666](https://www.github.com/mu88/ThisIsYourLife/commit/623c66621a99135e3512a01f928ba7aaf123ee81))
* **deps:** update all dependencies ([23c8452](https://www.github.com/mu88/ThisIsYourLife/commit/23c84526df76f6dea2748d2191555a7b58cad887))
* **deps:** update all dependencies ([1520e40](https://www.github.com/mu88/ThisIsYourLife/commit/1520e4042d90b4e9d2b3010f6079b0f1f59762ea))
* **deps:** update all dependencies ([01b2894](https://www.github.com/mu88/ThisIsYourLife/commit/01b289474e97146c8fe0cfefd17fe5bd15bb4ebd))
* **deps:** update all dependencies ([01bdd5f](https://www.github.com/mu88/ThisIsYourLife/commit/01bdd5fa71bc84a982a66087bc5da8ea6b554575))
* **deps:** update all dependencies ([01c20f5](https://www.github.com/mu88/ThisIsYourLife/commit/01c20f5b652b65330dd92beef39869470ba9d637))
* **deps:** update all dependencies ([76f3797](https://www.github.com/mu88/ThisIsYourLife/commit/76f3797825b365cf6861da4c2afb23d9e8341644))
* **deps:** update all dependencies ([40d41a4](https://www.github.com/mu88/ThisIsYourLife/commit/40d41a4f6f090b3bb5ea7c0993ff92149c478f29))
* **deps:** update all dependencies ([b8ce192](https://www.github.com/mu88/ThisIsYourLife/commit/b8ce192ed163ae5ada4806f25497064c15349fca))
* **deps:** update all dependencies ([26ee0e8](https://www.github.com/mu88/ThisIsYourLife/commit/26ee0e84788c750f78d0f9fdcf81ef9e6bcf99ad))
* **deps:** update all dependencies ([1bd1cb6](https://www.github.com/mu88/ThisIsYourLife/commit/1bd1cb69c667ac6021786f31127fb99e79e0f8a4))
* **deps:** update all dependencies ([ace1fad](https://www.github.com/mu88/ThisIsYourLife/commit/ace1fad9e20e0efb7c4146653c21a2dd7af868ec))
* **deps:** update all dependencies ([71c2286](https://www.github.com/mu88/ThisIsYourLife/commit/71c22860f92cf0e02885feaa66b7dff828500b80))
* **deps:** update all dependencies ([0a98b6e](https://www.github.com/mu88/ThisIsYourLife/commit/0a98b6ed858e9c10af9fc2edf9cf53e81f5a82db))
* **deps:** update all dependencies ([75532a7](https://www.github.com/mu88/ThisIsYourLife/commit/75532a7720ae7d129ce843b501c7d574ca6fa697))
* **deps:** update all dependencies ([a0f88a0](https://www.github.com/mu88/ThisIsYourLife/commit/a0f88a0b1efe6f7109ce55c182539ba28f26e2e2))
* **deps:** update all dependencies ([4c1efdb](https://www.github.com/mu88/ThisIsYourLife/commit/4c1efdbcadafc64cfae0adc73380cc3a96517457))
* **deps:** update all dependencies ([2cdbd35](https://www.github.com/mu88/ThisIsYourLife/commit/2cdbd356a3ee4cbddf8cf60b1e45ca4079ea0221))
* **deps:** update all dependencies ([802ef71](https://www.github.com/mu88/ThisIsYourLife/commit/802ef71a9b05feada471b1003ba477841ed3c52c))
* **deps:** update all dependencies ([440b258](https://www.github.com/mu88/ThisIsYourLife/commit/440b2586336553cea9984144c94a4b4817a76d5b))
* **deps:** update all dependencies ([7ca528f](https://www.github.com/mu88/ThisIsYourLife/commit/7ca528f7ed6fed2226230af1e92d61780e8bce4e))
* **deps:** update all dependencies ([ab8b949](https://www.github.com/mu88/ThisIsYourLife/commit/ab8b94941f2b45e89ea12d7a4f50627e14fd4931))
* **deps:** update all dependencies to 10.0.2 ([71c0602](https://www.github.com/mu88/ThisIsYourLife/commit/71c0602b546388d658ee4b7b4dad7bcffbd632a5))
* **deps:** update all dependencies to 10.0.6 ([dfeb58c](https://www.github.com/mu88/ThisIsYourLife/commit/dfeb58cf2378d3f369727c8e529ca61f34fac52d))
* **deps:** update dependency dotnet-coverage to v18.12.0 ([9d20eb2](https://www.github.com/mu88/ThisIsYourLife/commit/9d20eb234c091f97fdb1e1647a7b704175715360))
* **deps:** update mu88.Shared to version 6.0.0 for latest OTEL fixes ([c49a422](https://www.github.com/mu88/ThisIsYourLife/commit/c49a42239899a6818b295c3d66db15314f7e4ff0))
* **deps:** update mu88.Shared to version 7.0.0 for latest OTEL fixes ([d234767](https://www.github.com/mu88/ThisIsYourLife/commit/d234767b2cc0cfc6dcea05e3a31493e2d3aaf00e))
* **deps:** update mu88/common digest to 03ce929 ([f578cb7](https://www.github.com/mu88/ThisIsYourLife/commit/f578cb73c6c7082a70b42a64a869cf85b19210b0))
* **deps:** update mu88/common digest to 0d04e69 ([dbdcdf7](https://www.github.com/mu88/ThisIsYourLife/commit/dbdcdf71d5c32a22fb068ebdfac8d1ccd973a0c1))
* **deps:** update mu88/common digest to 349db23 ([f33c15b](https://www.github.com/mu88/ThisIsYourLife/commit/f33c15ba94666642fe8430932d2c00d2a2b6f8ee))
* **deps:** update mu88/common digest to 5d4855b ([5950903](https://www.github.com/mu88/ThisIsYourLife/commit/5950903b57d37550ececdfb46ce8d5a1345605af))
* **deps:** update mu88/common digest to 64260e9 ([911d464](https://www.github.com/mu88/ThisIsYourLife/commit/911d464c6a5c695ca685e705837fd598e1de4233))
* **deps:** update mu88/common digest to 88212c6 ([65caebe](https://www.github.com/mu88/ThisIsYourLife/commit/65caebec88e9520d24dba87a6d8bd122fad069ab))
* **deps:** update mu88/common digest to ab8025d ([f5213f7](https://www.github.com/mu88/ThisIsYourLife/commit/f5213f77835fa5206934f8bff4d09067ae4cf5c3))
* **deps:** update mu88/common digest to e3c88ef ([3914722](https://www.github.com/mu88/ThisIsYourLife/commit/391472205bed4691e3cd88dd3e695430de9dbebc))
* **deps:** upgrade mu88.Shared for latest OTel / health check updates ([06c2261](https://www.github.com/mu88/ThisIsYourLife/commit/06c226180576448cd48bed0f41be465654843ee9))
* **deps:** upgrade NUnit to 5.0.0 ([de5ac14](https://www.github.com/mu88/ThisIsYourLife/commit/de5ac149dba03f5919cb30a898163db20ded8b39))
* **deps:** use LibMan ([e5b72ca](https://www.github.com/mu88/ThisIsYourLife/commit/e5b72ca81113fc8c39d1cb9f0b111614a4bbfe40))
* **dev:** fix broken DotSettings links ([3d5b34e](https://www.github.com/mu88/ThisIsYourLife/commit/3d5b34ea06b6734c5b46706ad3d42d5b934e88af))

### Breaking Changes

* migrate to .NET 11 ([473965a](https://www.github.com/mu88/ThisIsYourLife/commit/473965a2487521ea6904d249df779fc741a9f171))

<a name="6.0.1"></a>
## [6.0.1](https://www.github.com/mu88/ThisIsYourLife/releases/tag/v6.0.1) (2025-12-13)

<a name="6.0.0"></a>
## [6.0.0](https://www.github.com/mu88/ThisIsYourLife/releases/tag/v6.0.0) (2025-12-12)

### ✨ Features

* upgrade to .NET 10 ([cee91b7](https://www.github.com/mu88/ThisIsYourLife/commit/cee91b7ceadc6a476c93c4f0fc774446b04ac865))

### 🔧 Chores

* switch to new SLNX format ([6a5ef6e](https://www.github.com/mu88/ThisIsYourLife/commit/6a5ef6e5f582fec062b6640be4d8cad2e3863707))
* **deps:** update all dependencies ([5f32b79](https://www.github.com/mu88/ThisIsYourLife/commit/5f32b7938a1ee95d65c40aca5276fa5f6683d396))
* **deps:** update all dependencies ([76f75ae](https://www.github.com/mu88/ThisIsYourLife/commit/76f75ae4520e53d6f588a9f268d8eca88e1313f6))
* **deps:** update all dependencies ([48261c6](https://www.github.com/mu88/ThisIsYourLife/commit/48261c63c93caecedb1543966a1a167a0f3d2ef5))
* **deps:** update package lock file ([6ed176e](https://www.github.com/mu88/ThisIsYourLife/commit/6ed176e9682488657598cc99f6d97ebed80c0896))

### Breaking Changes

* upgrade to .NET 10 ([cee91b7](https://www.github.com/mu88/ThisIsYourLife/commit/cee91b7ceadc6a476c93c4f0fc774446b04ac865))

<a name="5.0.0"></a>
## [5.0.0](https://www.github.com/mu88/ThisIsYourLife/releases/tag/v5.0.0) (2025-10-05)

### Bug Fixes

* dynamically determine user's home directory by using the corresponding environment variable ([d268e48](https://www.github.com/mu88/ThisIsYourLife/commit/d268e48e9989a54bd85a78c41a7a32ae9fbf68fa))

### Breaking Changes

* use GitHub Container Registry instead of Docker Hub ([03a4793](https://www.github.com/mu88/ThisIsYourLife/commit/03a479309bd784c1dc2b5124bdebd95b6e7136a6))

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

