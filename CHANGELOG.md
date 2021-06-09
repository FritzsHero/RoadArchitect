# Changelog
All notable changes to this RoadArchitect project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).


## [Version: x.x.x] - xxxx.xx.xx
### Improvements


## [Version: 2.0.0] - 2021.06.09
### Improvements
- Refactor of Editor code
- Removed "Buffers" from Scripts
- Refactor of IsApproximately
- Updated RoadUtility to use Unity 2019.2 API
- Added an option to change the desired height of the ramp
- Refactor of Profiling
- Added the ability to change default and selected gizmo color
- Added the ability to change the preview gizmo color for a new Node
- Added Offline Manual buttons to the Inspectors
- Removed unused using directives
- Fixed some issues with the quick help inspector
- Fixed an issue, which caused long names of physic materials
- Instantiating EdgeObjects as Prefabs
- Improved performance of CheckCreateSpecialLibraryDirs
- Improved performance of SplineNEditor Init function
- Reduced target casting in Editor scripts
- Refactored Init function of SplineNEditor
- Improved performance of RoadEditor Init function
- Improved performance of TerrainEditor Init function
- Improved performance of RoadSystemEditor Init function
- Improved performance of RoadIntersectionEditor Init function
- Refactored CheckLoadTexture into EditorUtilities
- Removed unused editor timer
- Unified DrawLine in EditorUtilities
- Unified SetupUniqueIdentifier
- Improved performance by reducing the frequency at which GetBasePath is called
- Added a warning for Tests
- Changed TestCodeCount to support more folders
- Fixed additional issues with physic material names
- Changed terrains to use heightmapResolution
- Moved all scripts into RoadArchitect namespace
- Added code summaries for many methods
- Renamed scripts
- Separated some scripts into new files
- Refactored Intersections Nullify
- Reorganized folder hierarchy
- Changed extension of library files to .rao
- Removed redundant directory queries
- Reworked building behavior of RoadArchitect
- Updated runtime usage of RoadArchitect
- Added unit test 7 and 8
- Refactored material assignments
- Extracted material assignments from inner loops
- Removed Editor limitation for node setup
- Removed unused variables
- Added unit test 9
- Refactored stop sign rigidbody creation
- Refactored DrawGizmos of SplineN
- Fixed terrain deformations when heightmapResolution was increased (Bug #21)
- Fixed update error when heightmapResolution was decreased (Bug #21)
- Simplified remove of old terrain histories
- Updated HelpWindow
- Updated supported folder locations
- Fixed regression with versions prior to 2018.1
- Improved cross platform compatibility
- Fixed StreetLight positions and updates
- Disabled horizontal collider on traffic lights
- Fixed traffic light positioning (Bug #14)
- Fixed update of intersection street light values


## [Version: 1.9_FH] - 2019.04.22
### Improvements
- Improved the Help Window with new Layout and Links
- Redone #if UNITY_EDITOR in Scripts
- Removed an empty function

### Changed
- Changed links, which redirected to Github, with the Github Wiki Link of the Master. Closed embeddedt/RoadArchitect/issues/6
- Outcommented unused using directives
- Changed LICENSE to an md file
- Minor Layout changes
- Deleted GSDEditorSkin, since it has no purpose at all

### Added
- Added missing private attribute on some Vars and Functions
- Added "Imports" Regions in the Scripts for better overview
- Added a "Report a Bug" MenuItem, which links to https://github.com/embeddedt/RoadArchitect/issues
- Added/Updated some regions


## [Version: 1.8_FH] - 2019.02.10
### Improvements
- Added some FH_Tag Optimizable as comments, since there is a way to further optimize the Code tagged by this
- Added a few Lines of comments to the code, to get a better idea of what the code does

### Changed
- Changed some Layouts of the Scripts
- Changed some Vars in Scripts to better reflect their purpose

### Added
- Added a few Regions to some Scripts

### Fixed
- Fixed embeddedt/RoadArchitect/issues/4


## [Version: 1.7.5a_FH] - 2019.01.31
### Changed
- Changed most Layouts of the Scripts
- Changed some Vars in Scripts to better reflect their purpose


## [Version 1.7] - 2018.07.16
### Changed
- Adds support for Unity 2018.x
- Updates to unit tests
- No further Informations given at this point


## [Version 1.6] - 2017.02.28
### Changed
- Initial Release
- No further Informations given at this point
