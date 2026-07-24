# SL-CustomObjects Developer Documentation (English)

Welcome to the official developer documentation for **SL-CustomObjects**, a specialized Unity-side editor framework designed to work seamlessly with **MapEditorReborn (MER)** (LabAPI & EXILED editions) for *SCP: Secret Laboratory*.

---

## 📖 Architecture & Design Overview

SL-CustomObjects allows you to design custom rooms, structures, and interactive maps in Unity and compile them into JSON schemas (`.json` for objects, `-Rigidbodies.json` for physics, and `-Teleports.json` for teleporters) that the server-side MapEditorReborn plugin can spawn in real-time.

```
+---------------------------------------+
|              Unity Editor             |
|   (Define GameObject Hierarchies)    |
+---------------------------------------+
                    |
                    v (Compile Schematic)
+---------------------------------------+
|        JSON Serialization Files       |
|    - [Name].json (All Blocks)         |
|    - [Name]-Rigidbodies.json          |
|    - [Name]-Teleports.json            |
+---------------------------------------+
                    |
                    v (Transfer to Server)
+---------------------------------------+
|          SCP: SL Game Server          |
|       (MapEditorReborn Spawns)        |
+---------------------------------------+
```

---

## 🛠️ Codebase & File Breakdown

### 1. Core Classes

#### 🔹 [Schematic.cs](file:///home/flaouve/Projects/scp%20sl/Map/Fla-SL-CustomObjects/Assets/DONT%20TOUCH/Scripts/Schematic.cs)
The root component of any custom map structure. It resides on the root GameObject and manages the compiling process.
* **`CompileSchematic()`**: Scans all children containing a `SchematicBlock` component, compiles them into list structures, extracts Animator/Animations into AssetBundles, handles Rigidbodies/Teleporters, serializes the data to JSON files, and optionally packages them into a `.zip` file for distribution.
* **`Update()`**: Enforces project constraints (e.g., preventing root scale changes and renaming directories to remove spaces).

#### 🔹 [SchematicBlock.cs](file:///home/flaouve/Projects/scp%20sl/Map/Fla-SL-CustomObjects/Assets/DONT%20TOUCH/Scripts/BlockComponents/SchematicBlock.cs)
The abstract base class for all custom objects. It inherits from Unity's `MonoBehaviour`.
* **`Compile(SchematicBlockData block)`**: Virtual method that extracts basic GameObject parameters (Name, instance ID, parent instance ID, Local Position, Local Rotation, Local Scale, and whether it's marked as `Static`).
* **`Decompile(...)`**: Virtual method allowing the system to reconstruct the Unity hierarchy from a serialized JSON file.

---

## 📖 Component Details & Usage Guide

Here are the details for the main components used to design custom maps in Unity and how they map to in-game objects:

---

### 1. 🟦 PrimitiveComponent (Geometric Shapes)
Represents standard Unity shapes (Cubes, Spheres, Cylinders, Capsules, Planes) spawned as server-side game primitives.
* **`Color`**: Defines the object's color. **Supports HDR (greater than 1.0) colors** for bright, neon-like emissive effects in-game.
* **`Collidable`**: If checked, players cannot pass through this object. If unchecked, players can pass through it.
* **`Visible`**: If checked, the object is visible. If unchecked, it behaves as an invisible wall.

---

### 2. 💡 LightComponent (Lights)
Spawns Point or Spot lights in-game to illuminate custom rooms.
* **`LightType`**: `Point` (emits light in all directions, like a light bulb) or `Spot` (emits light in a cone, like a flashlight).
* **`Color`**: Defines the light color. **Supports HDR colors** allowing light intensity to be boosted way beyond the default 0-255 range.
* **`Intensity`**: The strength/brightness of the light source.
* **`Range`**: The maximum reach distance of the light.
* **`ShadowType`**: Shadows can be set to `Soft`, `Hard`, or `None`.

---

### 3. 📝 TextComponent (Floating 3D Text)
Spawns floating 3D text objects (`TextToy`) in the game world.
* **How It Works:** Unity Inspector settings for **Color**, **FontSize**, and **Alignment** are automatically converted into standard HTML formatting tags (e.g., `<color=#FF0000FF>`, `<size=30>`, `<align=center>`) when compiling. The server passes this string directly to the game client, which renders the text with the specified color, size, and alignment.
* **Restoration (Decompile):** Importing a schematic back into Unity parses these tags and restores the corresponding TMPro component properties automatically.

---

### 4. 🖱️ InteractableComponent (Interaction Colliders)
Creates invisible trigger colliders that players can interact with by pressing/holding **E**.
* **`Shape`**: Trigger area shape (`Box` or `Sphere`).
* **`InteractionDuration`**: Holding time (seconds). Set to `0.0` for instant activation. Set to `3.0` if the player must hold E for 3 seconds.
* **`IsLocked`**: If true, the interaction is disabled.
* **`TargetObject`**: The **GameObject** (Cube or parent object) containing the **Animator** component to trigger.
* **`AnimationStateName`**: The name of the animation state (State) to play on interaction (e.g., `open`).
* **`AnimationStateName2`**: (Optional) If specified, the interaction will toggle between `AnimationStateName` and `AnimationStateName2` on consecutive interactions (e.g., toggling between `open` and `close`).

> [!TIP]
> For step-by-step setup of animatable objects and doors, check the [Animation Setup Guide (ANIMATION_TUTORIAL_EN.md)](file:///home/flaouve/Projects/scp%20sl/Map/Fla-SL-CustomObjects/Docs/ANIMATION_TUTORIAL_EN.md).

---

### 5. 🎒 PickupComponent (Dropped Items)
Defines spawnpoints for dropped items, keycards, or weapons.
* **`ItemType`**: The item to spawn (e.g., `KeycardO5`, `GunE11SR`).
* **`Locked` (Button Mode):** If checked, players cannot pick up the item. Instead, pressing/holding E on the item acts as a button trigger that fires schematic events.
* **`NumberOfUses` (Spawn/Use Limits):** Controls how many times players can pick up this item:
  * `1`: The item disappears after being picked up (default).
  * `5`: The item can be picked up 5 times (respawns 4 times after being picked up).
  * `-1`: **Infinite/Sonsuz** spawn point (never disappears).
* **`AttachmentsCode` (Weapon Attachments):** Pre-defines weapon attachment combinations.

#### 🔫 How to Get AttachmentsCode
1. Join your local or authorized SCP:SL game server.
2. Hold the weapon you want to customize in your hand.
3. Open the attachment menu (Tab key) and select your preferred scopes, barrels, stocks, etc.
4. Open the RA (Remote Admin) console (`~` or `é` key) and type the command:
   `forceatt`
5. The console will print a numeric code (e.g., `12456`).
6. Copy this code and paste it into the **`Attachments Code`** field of the `PickupComponent` in the Unity Inspector.

---

### 6. 📍 WaypointComponent (AI Navigation Nodes)
Positions navigation markers that bots (SCPs/Dummies) use to find paths.
* **`Priority`**: Navigation priority for bots (`0` to `255`). `255` is the highest priority. Bots will prioritize paths with higher waypoint priorities.

---

### 7. 🔧 WorkstationComponent (Weapon Workstations)
Spawns in-game workstation benches for weapon modifications.
* **`IsInteractable`**: Enables or disables interaction with the workbench.

---

## ⚙️ How to Compile a Schematic

1. Open your Unity Project.
2. Build your schematic structure by right-clicking in the **Hierarchy** -> **🛠️ MER Blocks** and adding components.
3. Keep all components nested under a single parent object containing the `Schematic` component.
4. Select the root object, navigate to its Inspector window, and click **Compile Schematic**.
5. Find the compiled JSON/AssetBundle output files inside your configured export path (or Desktop by default).
6. Copy these files to your SCP: SL server directory under:
   `LabAPI/configs/ProjectMER/Schematics/[YourSchematicName]/`
