using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAME_connection {
	[Serializable]
	public class TexturePair {
		private string templateName;
		private byte[] textureBytes;

		public TexturePair() { }

		public TexturePair(string templateName, byte[] textureBytes) {
			TemplateName = templateName;
			TextureBytes = textureBytes;
		}

		public string TemplateName { get => templateName; set => templateName = value; }
		public byte[] TextureBytes { get => textureBytes; set => textureBytes = value; }
	}

	[Serializable]
	public class Textures {
		private List<TexturePair> playerTextures;

		public Textures() { }

		public Textures(List<TexturePair> textures) {
			PlayerTextures = textures;
		}

		public List<TexturePair> PlayerTextures { get => playerTextures; set => playerTextures = value; }
	}
}
