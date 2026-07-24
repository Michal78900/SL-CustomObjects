# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.2.5]
- **Interactable:** Changed `TargetAnimator` property to `TargetObject` of type GameObject for simpler drag-and-drop operations in Unity Editor. The server-side component now dynamically resolves the Animator from the target object, its children, or its parents.
- **Waypoint:** Added Priority property support (0-255) for bot navigation node configurations.
- **Light:** Added HDR color serialization support (with ':' separated float format) to preserve emissive intensity in-game.
- **Text:** Added automatic rich text formatting tags conversion (FontSize, TextColor, TextAlignment) on Compile, and automatic tag parsing/stripping on Decompile.
- **Pickup:** Added `Locked` property support for converting normal pickups into interactable buttons.
- **Documentation:** Created comprehensive EN and TR guides and a dedicated step-by-step animation setup guide.

## [0.1.0]
- Initial release of the custom editor components for Unity 2022.3.22f1.
- Supported basic primitives, lights, custom text toys, interactable blocks, workstations, pickups, waypoints, and empty blocks.
- Integrated GitHub Actions automated packaging workflow.
