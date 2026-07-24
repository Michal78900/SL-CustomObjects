# SL-CustomObjects

Unity Editor tools and framework to create, manage, and compile custom schematics and maps for **MapEditorReborn (MER)** (available on LabAPI & EXILED) in *SCP: Secret Laboratory*.

This fork has been upgraded and optimized to work natively with **Unity 2022.3.22f1**.

---

## 📚 Documentation / Dokümantasyon

Comprehensive documentation detailing the architecture, every single component class, editor tools, and compile steps is available in both English and Turkish:

* [English Documentation (Docs/README_EN.md)](file:///home/flaouve/Projects/scp%20sl/Map/Fla-SL-CustomObjects/Docs/README_EN.md) - Deep dive into classes, serialization logic, and step-by-step schematic compiling.
* [Türkçe Dokümantasyon (Docs/README_TR.md)](file:///home/flaouve/Projects/scp%20sl/Map/Fla-SL-CustomObjects/Docs/README_TR.md) - Sınıfların açıklamaları, veri serileştirme mimarisi ve adım adım şablon derleme rehberi.

---

## ⚡ Main Features

* **MER Blocks Menu:** Easily spawn map components natively from Unity's right-click context menu.
* **Component-Based Architecture:** Every object has its corresponding logic class (Primitive, Light, Teleport, Text, Interactable, Waypoint, Workstation, Pickup, Locker, Camera).
* **Automatic Assembly:** Compiles complex hierarchy systems, animations into AssetBundles, rigidbodies, and teleport connections directly into MapEditorReborn-compatible JSON structure.
