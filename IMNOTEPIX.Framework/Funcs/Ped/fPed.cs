namespace IMNOTEPIX.Framework.Funcs.Ped
{
    using GTA;
    using GTA.Math;
    using GTA.Native;
    using System;
    using System.Collections.Generic;

    using Ped = GTA.Ped;
    public class fPed
    {
        public static Dictionary<int, Tuple<int, int>> GetPedVariationData(Ped _ped)
        {
            List<int> compIds = new List<int> { 0, 1, 2, 3, 4, 5, 6, 8, 9, 10, 11 };
            Dictionary<int, Tuple<int, int>> pedData = new Dictionary<int, Tuple<int, int>>();
            foreach (int id in compIds)
            {
                int drawID = Function.Call<int>(Hash.GET_PED_DRAWABLE_VARIATION, _ped, id);
                int textureID = Function.Call<int>(Hash.GET_PED_TEXTURE_VARIATION, _ped, id);
                Tuple<int, int> data = new Tuple<int, int>(drawID, textureID);
                pedData.Add(id, data);
            }
            if (pedData.Count > 0) return pedData;
            else return null;
        }

        public static Dictionary<int, Tuple<int, int>> GetPedPropData(Ped _ped)
        {
            List<int> compIds = new List<int> { 0, 1, 2 };
            Dictionary<int, Tuple<int, int>> pedData = new Dictionary<int, Tuple<int, int>>();
            foreach (int id in compIds)
            {
                int drawID = Function.Call<int>(Hash.GET_PED_PROP_INDEX, _ped, id);
                int textureID = Function.Call<int>(Hash.GET_PED_PROP_TEXTURE_INDEX, _ped, id);
                Tuple<int, int> data = new Tuple<int, int>(drawID, textureID);
                pedData.Add(id, data);
            }
            if (pedData.Count > 0) return pedData;
            else return null;
        }

        public static Ped CreatePed(Model model, Vector3 pos, float heading = 0)
        {
            return World.CreatePed(model, pos, heading);
        }


        public static Vehicle GetVehiclePedIsUsing(Ped ped)
        {
            return Function.Call<Vehicle>(Hash.GET_​VEHICLE_​PED_​IS_​USING, ped);
        }


        public static Vector3 GetPedBoneCoords(Ped ped, int boneId, Vector3 offset)
        {
            return Function.Call<Vector3>(Hash.GET_​PED_​BONE_​COORDS, ped, boneId, offset.X, offset.Y, offset.Z);
        }


        public static int GetPedBoneIndex(Ped ped, int boneId)
        {
            return Function.Call<int>(Hash.GET_​PED_​BONE_​INDEX, ped, boneId);
        }


        public static bool IsPedShootingInArea(Ped ped, float x1, float y1, float z1, float x2, float y2, float z2, bool p7 = false, bool p8 = true)
        {
            return Function.Call<bool>(Hash.IS_​PED_​SHOOTING_​IN_​AREA, ped, x1, y1, z1, x2, y2, z2, p7, p8);
        }

        public static bool HaveAllStreamingRequestsCompleted(Ped ped)
        {
            return Function.Call<bool>(Hash.HAVE_​ALL_​STREAMING_​REQUESTS_​COMPLETED, ped);
        }

        public static bool IsPedInAnyVehicle(Ped ped, bool atGetIn)
        {
            return Function.Call<bool>(Hash.IS_​PED_​IN_​ANY_​VEHICLE, ped, atGetIn);
        }

        public static bool ForcePedMotionState(Ped PedIndex, MotionStates state, bool shouldRestart = false, ExitStates exitstate = ExitStates.Default, bool ForceAIPreCameraUpdate = false)
        {
            return Function.Call<bool>(Hash.FORCE_PED_MOTION_STATE, PedIndex, state, shouldRestart, exitstate, ForceAIPreCameraUpdate);
        }


        public static void ApplyPedVariationData(Ped ped, Dictionary<int, Tuple<int, int>> data)
        {
            if (data == null) return;

            foreach (var kvp in data)
            {
                int componentId = kvp.Key;
                int drawable = kvp.Value.Item1;
                int texture = kvp.Value.Item2;

                Function.Call(
                    Hash.SET_PED_COMPONENT_VARIATION,
                    ped,
                    componentId,
                    drawable,
                    texture,
                    0
                );
            }
        }

        public static void ApplyPedPropData(Ped ped, Dictionary<int, Tuple<int, int>> data)
        {
            if (data == null) return;

            foreach (var kvp in data)
            {
                int propId = kvp.Key;
                int drawable = kvp.Value.Item1;
                int texture = kvp.Value.Item2;

                if (drawable < 0)
                {
                    // remove prop if none was equipped
                    Function.Call(Hash.CLEAR_PED_PROP, ped, propId);
                }
                else
                {
                    Function.Call(
                        Hash.SET_PED_PROP_INDEX,
                        ped,
                        propId,
                        drawable,
                        texture,
                        true
                    );
                }
            }
        }

        public static void SetMovementModeOverride(Ped ped, string name)
        {
            Function.Call(Hash.SET_​MOVEMENT_​MODE_​OVERRIDE, ped, name);
        }

        public static void SetPedCanLegIK(Ped ped, bool toggle)
        {
            Function.Call(Hash.SET_​PED_​CAN_​LEG_​IK, ped, toggle);
        }

        public static void SetPedPropIndex(Ped ped, int componentId, int drawableId, int TextureId, bool attach)
        {
            Function.Call(Hash.SET_​PED_​PROP_​INDEX, ped, componentId, drawableId, TextureId, attach);
        }

        public static void ClearAllPedProps(Ped ped, int p1 = 1)
        {
            Function.Call(Hash.CLEAR_​ALL_​PED_​PROPS, ped, p1);
        }

        public static void SetPedDefaultComponentVariation(Ped ped)
        {
            Function.Call(Hash.SET_​PED_​DEFAULT_​COMPONENT_​VARIATION, ped);
        }

        public static void SetPedComponentVariation(Ped ped, int componentId, int drawableId, int textureId, int paletteId)
        {
            Function.Call(Hash.SET_​PED_​COMPONENT_​VARIATION, ped, componentId, drawableId, textureId, paletteId);
        }

        public static void ForcePedAiAndAnimationUpdate(Ped ped, bool p1, bool p2)
        {
            Function.Call(Hash.FORCE_PED_AI_AND_ANIMATION_UPDATE, ped, p1, p2);
        }

        public static void SetPedAsCop(Ped ped, bool toggle)
        {
            Function.Call(Hash.SET_PED_AS_COP, ped, toggle);
        }

        [Obsolete("Pass CombatMovement enum instead of a raw int.")]
        public static void SetPedCombatMovement(Ped ped, int combatMovement)
        {
            Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, ped, combatMovement);
        }

        public static void SetPedCombatMovement(Ped ped, CombatMovement combatMovement)
        {
            Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, ped, (int)combatMovement);
        }

        public static void SetRelationshipBetweenGroups(fPed.RelationshipTypes relationship, int group1, int group2)
        {
            Function.Call(Hash.SET_RELATIONSHIP_BETWEEN_GROUPS, relationship, group1, group2);
        }

        public static void SetPedCombatAbility(Ped ped, fPed.CombatAbilityLevel abilityLevel)
        {
            Function.Call(Hash.SET_PED_COMBAT_ABILITY, ped, abilityLevel);
        }

        public static void SetPedCombatAttributes(Ped ped, fPed.CombatAttributes attributeID, bool toggle)
        {
            Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, attributeID, toggle);
        }

        public static void SetPedConfigFlag(Ped ped, int flagId, bool value = true)
        {
            Function.Call(Hash.SET_​PED_​CONFIG_​FLAG, ped, flagId, value);
        }

        public static void SetPedConfigFlag(Ped ped, fPed.PedConfigFlags configFlag, bool set)
        {
            Function.Call(Hash.SET_PED_CONFIG_FLAG, ped, configFlag, set);
        }


        public enum ExitStates
        {
            Default,
            CutsceneExit
        }
        public enum CombatAbilityLevel
        {
            Poor,
            Average,
            Professional
        }
        public enum CombatMovement
        {
            Stationary,
            Defensive,
            WillAdvance,
            WillRetreat
        };
        public enum RelationshipTypes
        {
            Companion = 0,
            Respect = 1,
            Like = 2,
            Neutral = 3,
            Dislike = 4,
            Hate = 5,
            Dead = 6,
            Pedestrians = 255
        }
        public enum MotionStates: uint
        {
            None = 4000413475u,
            Idle = 2423432979u,
            Walk = 3626484699u,
            Run = 4294436772u,
            Sprint = 3179812827u,
            CrouchIdle = 1140525470u,
            CrouchWalk = 147004056u,
            CrouchRun = 898879241u,
            DoNothing = 247561816u,
            AnimatedVelocity = 1427811395u,
            InVehicle = 2497303949u,
            Aiming = 1063765679u,
            DivingIdle = 1212730861u,
            DivingSwim = 2439938700u,
            SwimmingTreadWater = 3518960071u,
            Dead = 230360860u,
            StealthIdle = 1110276645u,
            StealthWalk = 69908130u,
            StealthRun = 4211833313u,
            Parachuting = 3133206795u,
            ActionModeIdle = 3661668572u,
            ActionModeWalk = 3532676775u,
            ActionModeRun = 834330132u,
            Jetpack = 1398696542u
        }
        public enum CombatAttributes
        {
            CA_INVALID = -1,
            CA_USE_COVER,
            CA_USE_VEHICLE,
            CA_DO_DRIVEBYS,
            CA_LEAVE_VEHICLES,
            CA_CAN_USE_DYNAMIC_STRAFE_DECISIONS,
            CA_ALWAYS_FIGHT,
            CA_FLEE_WHILST_IN_VEHICLE,
            CA_JUST_FOLLOW_VEHICLE,
            CA_PLAY_REACTION_ANIMS,
            CA_WILL_SCAN_FOR_DEAD_PEDS,
            CA_IS_A_GUARD,
            CA_JUST_SEEK_COVER,
            CA_BLIND_FIRE_IN_COVER,
            CA_AGGRESSIVE,
            CA_CAN_INVESTIGATE,
            CA_CAN_USE_RADIO,
            CA_CAN_CAPTURE_ENEMY_PEDS,
            CA_ALWAYS_FLEE,
            CA_CAN_TAUNT_IN_VEHICLE = 20,
            CA_CAN_CHASE_TARGET_ON_FOOT,
            CA_WILL_DRAG_INJURED_PEDS_TO_SAFETY,
            CA_REQUIRES_LOS_TO_SHOOT,
            CA_USE_PROXIMITY_FIRING_RATE,
            CA_DISABLE_SECONDARY_TARGET,
            CA_DISABLE_ENTRY_REACTIONS,
            CA_PERFECT_ACCURACY,
            CA_CAN_USE_FRUSTRATED_ADVANCE,
            CA_MOVE_TO_LOCATION_BEFORE_COVER_SEARCH,
            CA_CAN_SHOOT_WITHOUT_LOS,
            CA_MAINTAIN_MIN_DISTANCE_TO_TARGET,
            CA_CAN_USE_PEEKING_VARIATIONS = 34,
            CA_DISABLE_PINNED_DOWN,
            CA_DISABLE_PIN_DOWN_OTHERS,
            CA_OPEN_COMBAT_WHEN_DEFENSIVE_AREA_IS_REACHED,
            CA_DISABLE_BULLET_REACTIONS,
            CA_CAN_BUST,
            CA_IGNORED_BY_OTHER_PEDS_WHEN_WANTED,
            CA_CAN_COMMANDEER_VEHICLES,
            CA_CAN_FLANK,
            CA_SWITCH_TO_ADVANCE_IF_CANT_FIND_COVER,
            CA_SWITCH_TO_DEFENSIVE_IF_IN_COVER,
            CA_CLEAR_PRIMARY_DEFENSIVE_AREA_WHEN_REACHED,
            CA_CAN_FIGHT_ARMED_PEDS_WHEN_NOT_ARMED,
            CA_ENABLE_TACTICAL_POINTS_WHEN_DEFENSIVE,
            CA_DISABLE_COVER_ARC_ADJUSTMENTS,
            CA_USE_ENEMY_ACCURACY_SCALING,
            CA_CAN_CHARGE,
            CA_REMOVE_AREA_SET_WILL_ADVANCE_WHEN_DEFENSIVE_AREA_REACHED,
            CA_USE_VEHICLE_ATTACK,
            CA_USE_VEHICLE_ATTACK_IF_VEHICLE_HAS_MOUNTED_GUNS,
            CA_ALWAYS_EQUIP_BEST_WEAPON,
            CA_CAN_SEE_UNDERWATER_PEDS,
            CA_DISABLE_AIM_AT_AI_TARGETS_IN_HELIS,
            CA_DISABLE_SEEK_DUE_TO_LINE_OF_SIGHT,
            CA_DISABLE_FLEE_FROM_COMBAT,
            CA_DISABLE_TARGET_CHANGES_DURING_VEHICLE_PURSUIT,
            CA_CAN_THROW_SMOKE_GRENADE,
            CA_CLEAR_AREA_SET_DEFENSIVE_IF_DEFENSIVE_CANNOT_BE_REACHED = 62,
            CA_DISABLE_BLOCK_FROM_PURSUE_DURING_VEHICLE_CHASE = 64,
            CA_DISABLE_SPIN_OUT_DURING_VEHICLE_CHASE,
            CA_DISABLE_CRUISE_IN_FRONT_DURING_BLOCK_DURING_VEHICLE_CHASE,
            CA_CAN_IGNORE_BLOCKED_LOS_WEIGHTING,
            CA_DISABLE_REACT_TO_BUDDY_SHOT,
            CA_PREFER_NAVMESH_DURING_VEHICLE_CHASE,
            CA_ALLOWED_TO_AVOID_OFFROAD_DURING_VEHICLE_CHASE,
            CA_PERMIT_CHARGE_BEYOND_DEFENSIVE_AREA,
            CA_USE_ROCKETS_AGAINST_VEHICLES_ONLY,
            CA_DISABLE_TACTICAL_POINTS_WITHOUT_CLEAR_LOS,
            CA_DISABLE_PULL_ALONGSIDE_DURING_VEHICLE_CHASE,
            CA_DISABLE_ALL_RANDOMS_FLEE = 78,
            CA_WILL_GENERATE_DEAD_PED_SEEN_SCRIPT_EVENTS,
            CA_USE_MAX_SENSE_RANGE_WHEN_RECEIVING_EVENTS,
            CA_RESTRICT_IN_VEHICLE_AIMING_TO_CURRENT_SIDE,
            CA_USE_DEFAULT_BLOCKED_LOS_POSITION_AND_DIRECTION,
            CA_REQUIRES_LOS_TO_AIM,
            CA_CAN_CRUISE_AND_BLOCK_IN_VEHICLE,
            CA_PREFER_AIR_COMBAT_WHEN_IN_AIRCRAFT,
            CA_ALLOW_DOG_FIGHTING,
            CA_PREFER_NON_AIRCRAFT_TARGETS,
            CA_PREFER_KNOWN_TARGETS_WHEN_COMBAT_CLOSEST_TARGET,
            CA_FORCE_CHECK_ATTACK_ANGLE_FOR_MOUNTED_GUNS,
            CA_BLOCK_FIRE_FOR_VEHICLE_PASSENGER_MOUNTED_GUNS
        }
        public enum PedConfigFlags
        {
            _CPED_CONFIG_FLAG_0xC63DE95E = 1,
            CPED_CONFIG_FLAG_NoCriticalHits,
            CPED_CONFIG_FLAG_DrownsInWater,
            CPED_CONFIG_FLAG_DisableReticuleFixedLockon,
            _CPED_CONFIG_FLAG_0x37D196F4,
            _CPED_CONFIG_FLAG_0xE2462399,
            CPED_CONFIG_FLAG_UpperBodyDamageAnimsOnly,
            _CPED_CONFIG_FLAG_0xEDDEB838,
            _CPED_CONFIG_FLAG_0xB398B6FD,
            _CPED_CONFIG_FLAG_0xF6664E68,
            _CPED_CONFIG_FLAG_0xA05E7CA3,
            _CPED_CONFIG_FLAG_0xCE394045,
            CPED_CONFIG_FLAG_NeverLeavesGroup,
            _CPED_CONFIG_FLAG_0xCD8D1411,
            _CPED_CONFIG_FLAG_0xB031F1A9,
            _CPED_CONFIG_FLAG_0xFE65BEE3,
            CPED_CONFIG_FLAG_BlockNonTemporaryEvents,
            _CPED_CONFIG_FLAG_0x380165BD,
            _CPED_CONFIG_FLAG_0x07C045C7,
            PCF_AllowMedicsToAttend,
            PCF_DontAllowToBeDraggedOutOfVehicle = 26,
            PCF_GetOutUndriveableVehicle = 29,
            PCF_WillFlyThroughWindscreen = 32,
            PCF_HasHelmet = 34,
            PCF_DontTakeOffHelmet = 36,
            PCF_DontInfluenceWantedLevel = 42,
            PCF_DisableLockonToRandomPeds = 44,
            PCF_AllowLockonToFriendlyPlayers,
            PCF_DisableHornAudioWhenDead,
            PCF_IsAimingGun = 78,
            _CPED_CONFIG_FLAG_0x14D69875,
            _CPED_CONFIG_FLAG_0x40B05311,
            _CPED_CONFIG_FLAG_0x8B230BC5,
            _CPED_CONFIG_FLAG_0xC74E5842,
            _CPED_CONFIG_FLAG_0x9EA86147,
            _CPED_CONFIG_FLAG_0x674C746C,
            _CPED_CONFIG_FLAG_0x3E56A8C2,
            _CPED_CONFIG_FLAG_0xC144A1EF,
            _CPED_CONFIG_FLAG_0x0548512D,
            _CPED_CONFIG_FLAG_0x31C93909,
            _CPED_CONFIG_FLAG_0xA0269315,
            _CPED_CONFIG_FLAG_0xD4D59D4D,
            _CPED_CONFIG_FLAG_0x411D4420,
            _CPED_CONFIG_FLAG_0xDF4AEF0D,
            CPED_CONFIG_FLAG_ForcePedLoadCover,
            _CPED_CONFIG_FLAG_0x300E4CD3,
            _CPED_CONFIG_FLAG_0xF1C5BF04,
            _CPED_CONFIG_FLAG_0x89C2EF13,
            CPED_CONFIG_FLAG_VaultFromCover,
            _CPED_CONFIG_FLAG_0x02A852C8,
            _CPED_CONFIG_FLAG_0x3D9407F1,
            _CPED_CONFIG_FLAG_IsDrunk,
            PCF_ForcedAim,
            _CPED_CONFIG_FLAG_0xB942D71A,
            _CPED_CONFIG_FLAG_0xD26C55A8,
            PCF_OpenDoorArmIK,
            PCF_DontActivateRagdollFromVehicleImpact = 106,
            PCF_DontActivateRagdollFromBulletImpact,
            PCF_DontActivateRagdollFromExplosions,
            PCF_DontActivateRagdollFromFire,
            PCF_DontActivateRagdollFromElectrocution,
            _CPED_CONFIG_FLAG_0x83C0A4BF,
            _CPED_CONFIG_FLAG_0x0E0FAF8C,
            PCF_KeepWeaponHolsteredUnlessFired,
            PCF_ForceControlledKnockout,
            PCF_FallsOutOfVehicleWhenKilled,
            PCF_GetOutBurningVehicle,
            PCF_RunFromFiresAndExplosions = 118,
            PCF_TreatAsPlayerDuringTargeting,
            PCF_DisableMelee = 122,
            PCF_DisableUnarmedDrivebys,
            PCF_JustGetsPulledOutWhenElectrocuted,
            PCF_WillNotHotwireLawEnforcementVehicle = 126,
            PCF_WillCommandeerRatherThanJack,
            PCF_CanBeAgitated,
            PCF_ForcePedToFaceLeftInCover,
            PCF_ForcePedToFaceRightInCover,
            PCF_BlockPedFromTurningInCover,
            PCF_KeepRelationshipGroupAfterCleanUp,
            PCF_ForcePedToBeDragged,
            PCF_PreventPedFromReactingToBeingJacked,
            PCF_RemoveDeadExtraFarAway = 137,
            PCF_ArrestResult = 139,
            PCF_CanAttackFriendly,
            PCF_WillJackAnyPlayer,
            PCF_WillJackWantedPlayersRatherThanStealCar = 144,
            PCF_DisableLadderClimbing = 146,
            PCF_CowerInsteadOfFlee = 150,
            PCF_CanActivateRagdollWhenVehicleUpsideDown,
            PCF_AlwaysRespondToCriesForHelp,
            PCF_DisableBloodPoolCreation,
            PCF_ShouldFixIfNoCollision,
            PCF_CanPerformArrest,
            PCF_CanPerformUncuff,
            PCF_CanBeArrested,
            PCF_PlayerPreferFrontSeatMP = 159,
            PCF_DontEnterVehiclesInPlayersGroup = 167,
            PCF_CannotBeTargeted = 169,
            PCF_PreventAllMeleeTaunts = 169,
            PCF_ForceDirectEntry,
            PCF_AlwaysSeeApproachingVehicles,
            PCF_CanDiveAwayFromApproachingVehicles,
            PCF_AllowPlayerToInterruptVehicleEntryExit,
            PCF_OnlyAttackLawIfPlayerIsWanted,
            PCF_PedsJackingMeDontGetIn = 177,
            PCF_PedIgnoresAnimInterruptEvents = 179,
            PCF_IsInCustody,
            PCF_ForceStandardBumpReactionThresholds,
            PCF_LawWillOnlyAttackIfPlayerIsWanted,
            PCF_PreventAutoShuffleToDriversSeat = 184,
            PCF_UseKinematicModeWhenStationary,
            PCF_DisableHurt = 188,
            PCF_PlayerIsWeird,
            PCF_DoNothingWhenOnFootByDefault = 193,
            PCF_DontReactivateRagdollOnPedCollisionWhenDead = 198,
            PCF_DontActivateRagdollOnVehicleCollisionWhenDead,
            PCF_HasBeenInArmedCombat,
            PCF_Avoidance_Ignore_All = 202,
            PCF_Avoidance_Ignored_by_All,
            PCF_Avoidance_Ignore_Group1,
            PCF_Avoidance_Member_of_Group1,
            PCF_ForcedToUseSpecificGroupSeatIndex,
            PCF_DisableExplosionReactions = 208,
            PCF_WaitingForPlayerControlInterrupt = 210,
            PCF_ForcedToStayInCover,
            PCF_GeneratesSoundEvents,
            PCF_ListensToSoundEvents,
            PCF_AllowToBeTargetedInAVehicle,
            PCF_WaitForDirectEntryPointToBeFreeWhenExiting,
            PCF_OnlyRequireOnePressToExitVehicle,
            PCF_ForceExitToSkyDive,
            PCF_DisableExitToSkyDive = 221,
            PCF_DisablePedAvoidance = 226,
            PCF_ForceRagdollUponDeath,
            PCF_DisablePanicInVehicle = 229,
            PCF_AllowedToDetachTrailer,
            PCF_ForceSkinCharacterCloth = 240,
            PCF_LeaveEngineOnWhenExitingVehicles,
            PCF_PhoneDisableTextingAnimations,
            PCF_PhoneDisableTalkingAnimations,
            PCF_PhoneDisableCameraAnimations,
            PCF_DisableBlindFiringInShotReactions,
            PCF_AllowNearbyCoverUsage,
            PCF_CanAttackNonWantedPlayerAsLaw = 249,
            PCF_WillTakeDamageWhenVehicleCrashes,
            PCF_AICanDrivePlayerAsRearPassenger,
            PCF_PlayerCanJackFriendlyPlayers,
            PCF_AIDriverAllowFriendlyPassengerSeatEntry = 255,
            PCF_AllowMissionPedToUseInjuredMovement = 259,
            PCF_PreventUsingLowerPrioritySeats = 261,
            PCF_TeleportToLeaderVehicle = 268,
            PCF_Avoidance_Ignore_WeirdPedBuffer,
            PCF_DontBlipCop = 272,
            PCF_KillWhenTrapped = 275,
            PCF_AvoidTearGas = 279,
            PCF_DisableGoToWritheWhenInjured = 281,
            PCF_OnlyUseForcedSeatWhenEnteringHeliInGroup,
            PCF_DisableWeirdPedEvents = 285,
            PCF_ShouldChargeNow,
            PCF_DisableShockingEvents = 294,
            PCF_NeverReactToPedOnRoof = 296,
            PCF_DisableShockingDrivingOnPavementEvents = 299,
            PCF_ShouldThrowSmokeGrenadeNow,
            PCF_ForceInitialPeekInCover = 302,
            PCF_DisableJumpingFromVehiclesAfterLeader = 305,
            PCF_ShoutToGroupOnPlayerMelee = 311,
            PCF_IgnoredByAutoOpenDoors,
            PCF_ForceIgnoreMeleeActiveCombatant = 314,
            PCF_CheckLoSForSoundEvents,
            PCF_CanSayFollowedByPlayerAudio = 317,
            PCF_ActivateRagdollFromMinorPlayerContact,
            PCF_ForcePoseCharacterCloth = 320,
            PCF_HasClothCollisionBounds,
            PCF_DontBehaveLikeLaw = 324,
            PCF_DisablePoliceInvestigatingBody = 326,
            PCF_DisableWritheShootFromGround,
            PCF_LowerPriorityOfWarpSeats,
            PCF_DisableTalkTo,
            PCF_DontBlip,
            PCF_IgnoreLegIkRestrictions = 332,
            PCF_ForceNoTimesliceIntelligenceUpdate,
            PCF_AllowTaskDoNothingTimeslicing = 339,
            PCF_NotAllowedToJackAnyPlayers = 342,
            PCF_AlwaysLeaveTrainUponArrival = 345,
            PCF_OnlyWritheFromWeaponDamage = 347,
            PCF_UseSloMoBloodVfx,
            PCF_PreventDraggedOutOfCarThreatResponse = 350,
            PCF_ForceDeepSurfaceCheck = 356,
            PCF_DisableDeepSurfaceAnims,
            PCF_DontBlipNotSynced,
            PCF_IsDuckingInVehicle,
            PCF_PreventAutoShuffleToTurretSeat,
            PCF_DisableEventInteriorStatusCheck,
            PCF_TreatDislikeAsHateWhenInCombat = 364,
            PCF_OnlyUpdateTargetWantedIfSeen,
            PCF_AllowAutoShuffleToDriversSeat,
            PCF_PreventReactingToSilencedCloneBullets = 372,
            PCF_DisableInjuredCryForHelpEvents,
            PCF_NeverLeaveTrain,
            PCF_DontDropJetpackOnDeath,
            PCF_DisableAutoEquipHelmetsInBikes = 380,
            PCF_DisableAutoEquipHelmetsInAicraft,
            PCF_HasBareFeet = 389,
            PCF_UNUSED_REPLACE_ME_2,
            PCF_GoOnWithoutVehicleIfItIsUnableToGetBackToRoad,
            PCF_BlockDroppingHealthSnacksOnDeath,
            PCF_ForceThreatResponseToNonFriendToFriendMeleeActions = 394,
            PCF_DontRespondToRandomPedsDamage,
            PCF_AllowContinuousThreatResponseWantedLevelUpdates,
            PCF_KeepTargetLossResponseOnCleanup,
            PCF_PlayersDontDragMeOutOfCar,
            PCF_BroadcastRepondedToThreatWhenGoingToPointShooting,
            PCF_IgnorePedTypeForIsFriendlyWith,
            PCF_TreatNonFriendlyAsHateWhenInCombat,
            PCF_DontLeaveVehicleIfLeaderNotInVehicle,
            PCF_AllowMeleeReactionIfMeleeProofIsOn = 404,
            PCF_UseNormalExplosionDamageWhenBlownUpInVehicle = 407,
            PCF_DisableHomingMissileLockForVehiclePedInside,
            PCF_DisableTakeOffScubaGear,
            PCF_IgnoreMeleeFistWeaponDamageMult,
            PCF_LawPedsCanFleeFromNonWantedPlayer,
            PCF_ForceBlipSecurityPedsIfPlayerIsWanted,
            PCF_UseGoToPointForScenarioNavigation = 414,
            PCF_DontClearLocalPassengersWantedLevel,
            PCF_BlockAutoSwapOnWeaponPickups,
            PCF_ThisPedIsATargetPriorityForAI,
            PCF_IsSwitchingHelmetVisor,
            PCF_ForceHelmetVisorSwitch,
            PCF_UseOverrideFootstepPtFx = 421,
            PCF_DisableVehicleCombat,
            PCF_AllowBikeAlternateAnimations = 424,
            PCF_UseLockpickVehicleEntryAnimations = 426,
            PCF_IgnoreInteriorCheckForSprinting,
            PCF_SwatHeliSpawnWithinLastSpottedLocation,
            PCF_DisableStartEngine,
            PCF_IgnoreBeingOnFire,
            PCF_DisableTurretOrRearSeatPreference,
            PCF_DisableWantedHelicopterSpawning,
            PCF_UseTargetPerceptionForCreatingAimedAtEvents,
            PCF_DisableHomingMissileLockon,
            PCF_ForceIgnoreMaxMeleeActiveSupportCombatants,
            PCF_StayInDefensiveAreaWhenInVehicle,
            PCF_DontShoutTargetPosition,
            PCF_DisableHelmetArmor,
            PCF_PreventVehExitDueToInvalidWeapon = 441,
            PCF_IgnoreNetSessionFriendlyFireCheckForAllowDamage,
            PCF_DontLeaveCombatIfTargetPlayerIsAttackedByPolice,
            PCF_CheckLockedBeforeWarp,
            PCF_DontShuffleInVehicleToMakeRoom,
            PCF_GiveWeaponOnGetup,
            PCF_DontHitVehicleWithProjectiles,
            PCF_DisableForcedEntryForOpenVehiclesFromTryLockedDoor,
            PCF_FiresDummyRockets,
            PCF_DecoyPed = 451,
            PCF_HasEstablishedDecoy,
            PCF_BlockDispatchedHelicoptersFromLanding,
            PCF_DontCryForHelpOnStun,
            PCF_CanBeIncapacitated = 456,
            PCF_DontChangeTargetFromMelee = 458,
            PCF_RagdollFloatsIndefinitely = 460,
            PCF_BlockElectricWeaponDamage
        }
    }
}

