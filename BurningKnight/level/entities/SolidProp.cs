using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.events;
using BurningKnight.entity.item.util;
using BurningKnight.entity.projectile;
using BurningKnight.physics;
using BurningKnight.util.geometry;
using Lens.entity;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class SolidProp : Prop, CollisionFilterEntity {
		public override void PostInit() {
			base.PostInit();

			var collider = GetCollider();
			AddComponent(new RectBodyComponent(collider.X, collider.Y, collider.Width, collider.Height, BodyType.Static));
		}

		protected virtual Rectangle GetCollider() {
			return new Rectangle(0, 0, 32, 32);
		}

		public virtual bool ShouldCollide(Entity entity) {
			return !(entity is Creature);
		}
	}
}