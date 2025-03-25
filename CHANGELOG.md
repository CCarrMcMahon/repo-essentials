# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.3.5] - 2025-03-25

### Added

-   Created this `CHANGELOG.md` file to track releases better ([#20](https://github.com/CCarrMcMahon/repo-essentials/issues/20)).
-   Introduced a new step in the build script to include this file in the release package.

### Changed

-   Reduced the default value of `[Chat] LineSpacing` to `-45` to avoid overlap with large characters such as brackets.
-   Refactored the config table in `README.md` to better show the range of valid values.
-   Renamed the `CurrentCulture` class to `CurrencyCulture` so it matches the file and patch name.

### Fixed

-   Configuration adjusted to work with RepoConfig ([#18](https://github.com/CCarrMcMahon/repo-essentials/issues/18)).
    -   **Note:** A game restart is currently required after changing config values as they are injected at launch.

[unreleased]: https://github.com/CCarrMcMahon/repo-essentials/compare/v0.3.5...HEAD
[0.3.5]: https://github.com/CCarrMcMahon/repo-essentials/compare/v0.3.4...v0.3.5
[0.3.4]: https://github.com/CCarrMcMahon/repo-essentials/compare/v0.3.3...v0.3.4
[0.3.3]: https://github.com/CCarrMcMahon/repo-essentials/compare/v0.3.2...v0.3.3
[0.3.2]: https://github.com/CCarrMcMahon/repo-essentials/compare/v0.3.1...v0.3.2
[0.3.1]: https://github.com/CCarrMcMahon/repo-essentials/compare/v0.3.0...v0.3.1
[0.3.0]: https://github.com/CCarrMcMahon/repo-essentials/compare/v0.2.0...v0.3.0
[0.2.0]: https://github.com/CCarrMcMahon/repo-essentials/releases/tag/v0.2.0
