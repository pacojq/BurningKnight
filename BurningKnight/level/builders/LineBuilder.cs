using System;
using System.Collections.Generic;
using BurningKnight.level.rooms;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.util;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.level.builders {
	public class LineBuilder : RegularBuilder {
		private float Direction;

		public LineBuilder() {
			Direction = Rnd.Angle();
		}
		
		public LineBuilder SetAngle(float Angle) {
			Direction = Angle % 360f;
			return this;
		}

		private bool PlaceBoss(List<RoomDef> Init, RoomDef R) {
			var a = Direction;
			var i = 0;
						
			while (true) {
				var an = PlaceRoom(Init, R, Boss, a);
							
				if ((int) an != -1) {
					break;
				}

				i++;

				if (i > 36) {
					return false;
				}
							
				a += 10;
			}

			if (Granny != null) {
				a = Rnd.Angle();
				i = 0;

				while (true) {
					var an = PlaceRoom(Init, Boss, Granny, a);

					if ((int) an != -1) {
						break;
					}

					i++;

					if (i > 72) {
						return false;
					}

					a += 5;
				}
			}

			if (OldMan != null) {
				a = Rnd.Angle();
				i = 0;

				while (true) {
					var an = PlaceRoom(Init, Boss, OldMan, a);

					if ((int) an != -1) {
						break;
					}

					i++;

					if (i > 72) {
						return false;
					}

					a += 5;
				}
			}

			return true;
		}

		public override List<RoomDef> Build(List<RoomDef> Init) {
			SetupRooms(Init);

			if (Entrance == null) {
				Log.Error("No entrance!");
				return null;
			}

			var Branchable = new List<RoomDef>();
			
			Entrance.SetSize();
			Entrance.SetPos(0, 0);
			Branchable.Add(Entrance);

			if (MultiConnection.Count == 0) {
				PlaceRoom(Init, Entrance, Exit, Rnd.Angle());

				if (Boss != null && !PlaceBoss(Init, Exit)) {
					return null;
				}
				
				return Init;
			}

			var RoomsOnPath = (int) (MultiConnection.Count * PathLength) + Rnd.Chances(PathLenJitterChances);
			RoomsOnPath = Math.Min(RoomsOnPath, MultiConnection.Count);
			RoomDef Curr = Entrance;
			var PathTunnels = ArrayUtils.Clone(PathTunnelChances);

			for (var I = 0; I <= RoomsOnPath; I++) {
				if (I == RoomsOnPath && Exit == null) continue;

				var Tunnels = Rnd.Chances(PathTunnels);

				if (Tunnels == -1) {
					PathTunnels = ArrayUtils.Clone(PathTunnelChances);
					Tunnels = Rnd.Chances(PathTunnels);
				}

				PathTunnels[Tunnels]--;

				if (I != 0 && Run.Depth != 0)
					for (var J = 0; J < Tunnels; J++) {
						var T = RoomRegistry.Generate(RoomType.Connection, LevelSave.BiomeGenerated);

						if (Math.Abs(PlaceRoom(Init, Curr, T, Direction + Rnd.Float(-PathVariance, PathVariance)) - (-1)) < 0.01f) {
							return null;
						}

						Branchable.Add(T);
						Init.Add(T);
						Curr = T;
					}

				var R = I == RoomsOnPath ? Exit : MultiConnection[I];


				if (Math.Abs(PlaceRoom(Init, Curr, R, Direction + Rnd.Float(-PathVariance, PathVariance)) - (-1)) < 0.01f) {
					return null;
				}
				
				if (R == Exit && Boss != null && !PlaceBoss(Init, R)) {
					return null;
				}

				Branchable.Add(R);
				Curr = R;
			}

			var RoomsToBranch = new List<RoomDef>();

			for (var I = RoomsOnPath; I < MultiConnection.Count; I++) {
				RoomsToBranch.Add(MultiConnection[I]);
			}

			RoomsToBranch.AddRange(SingleConnection);

			WeightRooms(Branchable);

			if (!CreateBranches(Init, Branchable, RoomsToBranch, BranchTunnelChances)) {
				return null;
			}
			
			FindNeighbours(Init);

			foreach (var R in Init) {
				foreach (var N in R.Neighbours) {
					if (!N.Connected.ContainsKey(R) && Rnd.Float() < ExtraConnectionChance) {
						R.ConnectWithRoom(N);
					}
				}
			}

			return Init;
		}
	}
}