using System;
using System.Collections.Generic;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.npc.dungeon;
using BurningKnight.level.rooms.special;
using BurningKnight.save;
using BurningKnight.util.geometry;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.darkmarket {
	public class DarkMarketRoom : SpecialRoom {
		public override void Paint(Level level) {
			var points = new List<Vector2>();
			var a = false;
			
			if (Rnd.Chance()) {
				a = true;
				points.Add(new Vector2(Left + 3.5f, Top + 3.5f) * 16);
			}
			
			if (Rnd.Chance()) {
				a = true;
				points.Add(new Vector2(Right - 2.5f, Bottom - 3f) * 16);
			}

			if (Rnd.Chance()) {
				a = true;
				points.Add(new Vector2(Right - 2.5f, Top + 3.5f) * 16);
			}

			if (!a || Rnd.Chance()) {
				points.Add(new Vector2(Left + 3.5f, Bottom - 3f) * 16);
			}

			var types = new List<byte> {
				/*0, 1, 2, 3,*/ 4 /*5, 6, 7, 8*/
			};

			if (GlobalSave.IsTrue(ShopNpc.Roger)) {
				types.Add(0);
			}

			if (GlobalSave.IsTrue(ShopNpc.Boxy)) {
				types.Add(1);
			}

			if (GlobalSave.IsTrue(ShopNpc.Snek)) {
				types.Add(2);
			}

			if (GlobalSave.IsTrue(ShopNpc.Vampire)) {
				types.Add(5);
			}

			if (GlobalSave.IsTrue(ShopNpc.Nurse)) {
				types.Add(6);
			}

			if (GlobalSave.IsTrue(ShopNpc.Elon)) {
				types.Add(7);
			}

			if (GlobalSave.IsTrue(ShopNpc.Duck)) {
				types.Add(8);
			}

			foreach (var p in points) {
				var i = Rnd.Int(types.Count);
				var tp = types[i];
				types.RemoveAt(i);

				switch (tp) {
					case 0: {
						Roger.Place(p, level.Area);
						break;
					}
					
					case 1: {
						Boxy.Place(p, level.Area);
						break;
					}
					
					case 2: {
						Snek.Place(p, level.Area);
						break;
					}
					
					case 3: {
						Gobetta.Place(p, level.Area);
						break;
					}
					
					case 4: {
						TrashGoblin.Place(p, level.Area);
						break;
					}
					
					case 5: {
						Vampire.Place(p, level.Area);
						break;
					}
					
					case 6: {
						Nurse.Place(p, level.Area);
						break;
					}
					
					case 7: {
						DungeonElon.Place(p, level.Area);
						break;
					}
					
					case 8: {
						DungeonDuck.Place(p, level.Area);
						break;
					}
				}

				if (types.Count == 0) {
					break;
				}
			}
		}

		public override void SetupDoors(Level level) {
			foreach (var door in Connected.Values) {
				door.Type = DoorPlaceholder.Variant.Head;
			}
		}

		public override bool CanConnect(RoomDef R, Dot P) {
			if (P.X == Left || P.X == Right || P.Y == Top) {
				return false;
			}
			
			return base.CanConnect(R, P);
		}

		public override int GetMinWidth() {
			return 14;
		}

		public override int GetMinHeight() {
			return 12;
		}

		public override int GetMaxWidth() {
			return 15;
		}

		public override int GetMaxHeight() {
			return 13;
		}
	}
}