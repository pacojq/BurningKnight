using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.entity.lamp;
using Lens.util.math;

namespace BurningKnight.entity.component {
	public class LampComponent : ItemComponent {
		public Lamp Lamp;
		
		protected override void OnItemSet() {
			Item?.Use(Entity);

			if (Lamp != null) {
				Lamp.Done = true;
			}
			
			if (Item != null) {
				Lamp = new Lamp {
					Owner = (Player) Entity,
					Item = Item
				};
				
				Entity.Area.Add(Lamp);
				Lamp.Center = Entity.Center;

				creature.BurningKnight knight;
				
				if (Entity.Area.Tags[Tags.BurningKnight].Count == 0) {
					knight = new creature.BurningKnight();
					Entity.Area.Add(knight);
				} else {
					var list = Entity.Area.Tags[Tags.BurningKnight];
					knight = (creature.BurningKnight) list[Random.Int(list.Count)];
				}

				knight.SetLamp(Item);
			}
		}

		protected override bool ShouldReplace(Item item) {
			return item.Type == ItemType.Lamp;
		}
	}
}