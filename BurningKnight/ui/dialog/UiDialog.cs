using System;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.ui.str;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.input;
using Lens.util;
using Lens.util.camera;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui.dialog {
	public class UiDialog : FrameRenderer {
		public Entity Owner;

		private TextureRegion triangle;
		public UiString Str;

		public bool Saying { get; private set; }
		public bool DoneSaying { get; private set; }

		public Func<bool> OnEnd;

		private string toSay;
		
		public override void Init() {
			base.Init();
			
			Width = 4;
			Height = 4;
				
			Setup("ui", "dialog_");
			triangle = CommonAse.Ui.GetSlice("dialog_tri");
			
			Str = new UiString(Font.Small);
			Area.Add(Str);

			Depth = 2;
			Str.Paused = true;
			Str.Depth = Depth + 1;
			Str.WidthLimit = 172;
			Str.Visible = false;
			Str.AlwaysVisible = false;
			
			Str.FinishedTyping += s => {
				DoneSaying = true;
			};
			
			Tint.A = 0;
		}

		public override void Destroy() {
			base.Destroy();
			Str.Done = true;
		}

		public void Say(string s) {
			if (Str == null) {
				toSay = s;
				return;
			}
			
			if (!Saying) {
				Tween.To(255, 0, x => Tint.A = (byte) x, 0.3f);
			}
			
			Saying = true;
			Str.Width = 4;
			Str.Height = 4;
			Str.Label = s;
		}

		public override void Render() {
			if (!Engine.Instance.State.Paused) {
				base.Render();
				Str?.Render();
			}
		}

		public override void RenderFrame() {
			if (Tint.A == 0) {
				return;
			}
			
			base.RenderFrame();
			
			Graphics.Color = Tint;
			Graphics.Render(triangle, Position + new Vector2((Width - triangle.Width) / 2f, Height - 1f));
			Graphics.Color = ColorUtils.WhiteColor;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Camera.Instance == null) {
				return;
			}
			
			Position = Camera.Instance.CameraToUi(new Vector2(Owner.CenterX, Owner.Y - 4));
			Height += (Str.Height + 12 - Height) * dt * 10;
			Width += (Str.Width + 16 - Width) * dt * 10;
			X -= Width / 2;
			Y -= Height;

			Str.Tint = Tint;
			Str.Position = Position + new Vector2(8, 4);

			if (toSay != null) {
				Say(toSay);
				toSay = null;
			}
		}

		public void Finish() {
			DoneSaying = false;

			if (OnEnd == null || OnEnd.Invoke()) {
				Close();
			}
		}
		
		public void Close(Action callback = null) {
			Saying = false;
			DoneSaying = false;
			
			Tween.To(0, 255, x => Tint.A = (byte) x, 0.3f).OnEnd = () => {
				Str.Width = 4;
				Str.Height = 4;
				Width = 4;
				Height = 4;
				callback?.Invoke();
			};
		}
	}
}