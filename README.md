![render](media/render.gif)

A custom 3D rendering and game application built in C# using the OpenTK library. The project features a modular, object-oriented architecture designed to handle custom GLSL shaders, scene management, and entity logic, alongside UI integration.

---

## Core Architecture & Features

* **OpenTK Integration:** Leverages OpenGL for hardware-accelerated 3D graphics and rendering directly within a .NET environment.
* **Custom Shader Pipeline:** Supports custom vertex and fragment shaders (stored in `/shaders`) for advanced graphical effects and material rendering.
* **Scene Management:** Uses a dedicated scene system (`/scenes`) to handle different game states, transitions, and rendering loops cleanly.
* **Object-Oriented Entity System:** Game entities and logic are separated into isolated classes (`/objects`) and governed by strict contracts (`/interfaces`).
* **UI & Forms:** Integrates standard windowing or UI forms (`/forms`) alongside the OpenGL context for tooling or overlays.

---

## Structure

* **`/assets`** - Static resources including textures, 3D models, and audio files.
* **`/documents`** - Project documentation, planning, and design notes.
* **`/forms`** - UI components, window definitions, and interface layouts.
* **`/interfaces`** - C# interfaces defining the contracts for game objects, rendering logic, and core systems.
* **`/objects`** - Classes for in-game entities, props, and logic controllers.
* **`/packages`** - Local NuGet package dependencies.
* **`/scenes`** - Logic for distinct application states (e.g., Main Menu, Level 1, Engine View).
* **`/shaders`** - GLSL files containing custom graphical shading logic.
* **`/vendor`** - Third-party libraries and external dependencies.

---

### Prerequisites
* **IDE:** Visual Studio 2019/2022 or JetBrains Rider.
* **Framework:** .NET Framework or .NET Core (matching the `skystride.csproj` target).
* **Hardware:** A GPU with modern OpenGL support.
