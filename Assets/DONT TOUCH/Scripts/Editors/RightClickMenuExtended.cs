using UnityEditor;
using UnityEngine;

public class RightClickMenuExtended
{
	[MenuItem("GameObject/🛠️ MER Blocks/Empty (Schematic)", false, -2)]
	private static void CreateEmptyOrSchematic(MenuCommand menuCommand)
	{
		GameObject parent = Selection.activeGameObject;

		if (menuCommand.context as GameObject != null)
			parent = menuCommand.context as GameObject;

		if (parent == null)
		{
			CreateBlock(menuCommand, "Assets/Resources/Blocks/Schematic.prefab");
		}
		else
		{
			CreateBlock(menuCommand, "Assets/Resources/Blocks/Empty.prefab");
		}
	}

	#region Primitives
	[MenuItem("GameObject/🛠️ MER Blocks/Primitives/Cube", false, -1)]
	private static void CreateCube(MenuCommand menuCommand) => CreatePrimitive(menuCommand, PrimitiveType.Cube);

	[MenuItem("GameObject/🛠️ MER Blocks/Primitives/Sphere", false, -1)]
	private static void CreateSphere(MenuCommand menuCommand) => CreatePrimitive(menuCommand, PrimitiveType.Sphere);

	[MenuItem("GameObject/🛠️ MER Blocks/Primitives/Capsule", false, -1)]
	private static void CreateCapsule(MenuCommand menuCommand) => CreatePrimitive(menuCommand, PrimitiveType.Capsule);

	[MenuItem("GameObject/🛠️ MER Blocks/Primitives/Cylinder", false, -1)]
	private static void CreateCylinder(MenuCommand menuCommand) => CreatePrimitive(menuCommand, PrimitiveType.Cylinder);

	[MenuItem("GameObject/🛠️ MER Blocks/Primitives/Plane", false, -1)]
	private static void CreatePlane(MenuCommand menuCommand) => CreatePrimitive(menuCommand, PrimitiveType.Plane);

	[MenuItem("GameObject/🛠️ MER Blocks/Primitives/Quad", false, -1)]
	private static void CreateQuad(MenuCommand menuCommand) => CreatePrimitive(menuCommand, PrimitiveType.Quad);

	private static void CreatePrimitive(MenuCommand menuCommand, PrimitiveType primitiveType) => CreateBlock(menuCommand, $"Assets/Resources/Blocks/Primitives/{primitiveType}.prefab");
	#endregion

	#region Lights
	[MenuItem("GameObject/🛠️ MER Blocks/Lights/Directional", false, -1)]
	private static void CreateDirectionalLight(MenuCommand menuCommand) => CreateLight(menuCommand, LightType.Directional);

	[MenuItem("GameObject/🛠️ MER Blocks/Lights/Point", false, -1)]
	private static void CreatePointLight(MenuCommand menuCommand) => CreateLight(menuCommand, LightType.Point);

	[MenuItem("GameObject/🛠️ MER Blocks/Lights/Spot", false, -1)]
	private static void CreateSpotLight(MenuCommand menuCommand) => CreateLight(menuCommand, LightType.Spot);

	[MenuItem("GameObject/🛠️ MER Blocks/Lights/Area", false, -1)]
	private static void CreateAreaLight(MenuCommand menuCommand) => CreateLight(menuCommand, LightType.Area);

	private static void CreateLight(MenuCommand menuCommand, LightType lightType) => CreateBlock(menuCommand, $"Assets/Resources/Blocks/Lights/{lightType} Light.prefab");
	#endregion

	[MenuItem("GameObject/🛠️ MER Blocks/Pickup", false, -1)]
	private static void CreatePickup(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Pickup.prefab");

	[MenuItem("GameObject/🛠️ MER Blocks/Workstation", false, -1)]
	private static void CreateWorkstation(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Workstation.prefab");

	// [MenuItem("GameObject/🛠️ MER Blocks/Teleport", false, -1)]
	// private static void CreateTeleport(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Teleporter.prefab");

	[MenuItem("GameObject/🛠️ MER Blocks/Text", false, -1)]
	private static void CreateText(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Text.prefab");

	[MenuItem("GameObject/🛠️ MER Blocks/Interactable", false, -1)]
	private static void CreateInteractable(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Interactable.prefab");

	[MenuItem("GameObject/🛠️ MER Blocks/Waypoint", false, -1)]
	private static void CreateWaypoint(MenuCommand menuCommand) => CreateBlock(menuCommand, "Assets/Resources/Blocks/Waypoint.prefab");

	private static void CreateBlock(MenuCommand menuCommand, string prefabPath)
	{
		GameObject instance = SchematicBlock.Create<GameObject>(prefabPath);
		if (instance == null)
			return;

		GameObject parent = Selection.activeGameObject;

		if (menuCommand.context as GameObject != null)
			parent = menuCommand.context as GameObject;

		GameObjectUtility.SetParentAndAlign(instance, parent);

		Undo.RegisterCreatedObjectUndo(instance, $"Create {instance.name}");

		Selection.activeGameObject = instance;
	}
}
