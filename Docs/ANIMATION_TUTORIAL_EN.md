# 🎬 MER Animation Setup & Interaction Guide (English)

This guide provides a step-by-step walkthrough on how to set up animatable objects (e.g., doors, elevators, moving platforms) in Unity and configure them to be triggered when players press the **E** key in-game.

---

## 🏗️ 1. Unity Animator & Animation Setup

To make your animated object (e.g., a custom door) function properly, the Animator Controller and Animation clips must be correctly structured:

1. **Adding an Animator:**
   * Select the visual object you want to animate (e.g., `Kapi_Mesh`).
   * In the Inspector panel, click **Add Component -> Animator**.
2. **Creating an Animator Controller:**
   * Right-click in the Project window: **Create -> Animator Controller**. Name it `Kapi_Controller`.
   * Drag and drop this `Kapi_Controller` into the **Controller** slot of your Animator component.
3. **Creating Animation Clips:**
   * Select your object and open the **Animation** window (Window -> Animation -> Animation).
   * Click **Create** to make the first clip. Name it exactly **`open`** (the movement of the door opening).
   * Create a second clip and name it exactly **`close`** (the movement of the door closing).
   * Animate the transform values and save your clips.
4. **Adding States to the Animator:**
   * Open the **Animator** window.
   * Verify that both `open` and `close` animation clips exist as states (boxes) in this window.
   * **Crucial:** The state names in the Animator graph must be exactly `open` and `close` (lowercase).

---

## 🖱️ 2. Interactable (Trigger Collider) Configuration

To allow players to trigger this animation by pressing E, we need to add an interaction zone.

> [!WARNING]
> **Common Cause of Crashes:** Do NOT add both `PrimitiveComponent` and `InteractableComponent` scripts to the same GameObject in Unity. Spawning a schematic with duplicate Object IDs on the same transform will crash the server.

### Recommended Hierarchy:
```
── 🛠️ Root Schematic GameObject (Schematic Component)
   ├── 🟦 Kapi_Mesh (PrimitiveComponent + Animator)  <-- Visual mesh & animation only
   └── 🖱️ Kapi_Trigger (InteractableComponent)       <-- Invisible interaction collider only
```

### Setup Steps:
1. Right-click in the Hierarchy window: **`🛠️ MER Blocks -> Interactable`** to spawn a new interaction block.
2. Position the newly created trigger box in front of or next to your door (where the player will stand to press E).
3. Select the trigger and configure the following fields in the Inspector:
   * **`Target Object`**: Drag and drop the object containing the Animator component (`Kapi_Mesh` or any parent/child object).
     * *The server uses a dynamic lookup mechanism to automatically find the Animator on the target object, its children, or its parents.*
   * **`Animation State Name`**: The exact state name to play on first interaction: **`open`**
   * **`Animation State Name 2`**: (Optional) The state name to play on the next interaction (for toggling back and forth): **`close`**
   * **`Interaction Duration`**: Set to `0` for instant activation. For a timed circle-bar interaction, enter the duration in seconds (e.g., `3.0`).

---

## 📦 3. Compiling & Exporting to Server

1. Select your root schematic object and click **`Compile Schematic`** in the Inspector.
2. The compiler will automatically bundle your animations into an uzantısız (extensionless) **AssetBundle** file matching your schematic name (or child name, e.g., `Cube`).
3. Find the compiled folder (e.g., `test3/`) on your Desktop and copy it to your server under:
   `LabAPI/configs/ProjectMER/Schematics/test3/`
4. Confirm that the folder contains:
   * `test3.json`
   * `Cube` (Extensionless AssetBundle file)
