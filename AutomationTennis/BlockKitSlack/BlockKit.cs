using System.Text.Json.Serialization;

namespace AutomationTennis.BlockKitSlack
{

    public class BlockKit
    {
        [JsonPropertyName("blocks")]
        public List<Block> Blocks { get; set; } = new List<Block>();
    }

    [JsonDerivedType(typeof(HeaderBlock))]
    [JsonDerivedType(typeof(ContextBlock))]
    [JsonDerivedType(typeof(ImageBlock))]
    [JsonDerivedType(typeof(DividerBlock))]
    public abstract class Block
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        public Block(string type) { 
            Type = type;
        }
    }

    public class HeaderBlock : Block
    {
        [JsonPropertyName("text")]
        public PlainText Text { get; set; } = new PlainText();

        public HeaderBlock() : base("header")
        {
        }
    }

    public class ContextBlock : Block
    {
        [JsonPropertyName("elements")]
        public List<MrkdwnText> Elements { get; set; } = new List<MrkdwnText>();

        public ContextBlock() : base("context")
        {
        }
    }

    public class ImageBlock : Block
    {
        [JsonPropertyName("block_id")]
        public string BlockId { get; set; } = string.Empty;

        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; } = string.Empty;

        [JsonPropertyName("alt_text")]
        public string AltText { get; set; } = string.Empty;

        public ImageBlock() : base("image")
        {

        }
    }

    public class DividerBlock : Block
    {
        public DividerBlock() : base("divider")
        {

        }
    }

    public class PlainText
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "plain_text";

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("emoji")]
        public bool Emoji { get; set; } = true;
    }

    public class MrkdwnText
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "mrkdwn";

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }

}
