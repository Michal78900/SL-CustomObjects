using System;

[Flags]
public enum DoorPermissionFlags : ushort
{
	None = 0,
	All = 65535, // 0xFFFF
	Checkpoints = 1,
	ExitGates = 2,
	Intercom = 4,
	AlphaWarhead = 8,
	ContainmentLevelOne = 16, // 0x0010
	ContainmentLevelTwo = 32, // 0x0020
	ContainmentLevelThree = 64, // 0x0040
	ArmoryLevelOne = 128, // 0x0080
	ArmoryLevelTwo = 256, // 0x0100
	ArmoryLevelThree = 512, // 0x0200
	ScpOverride = 1024, // 0x0400
}