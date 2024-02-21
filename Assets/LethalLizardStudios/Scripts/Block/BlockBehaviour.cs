using UnityEngine;

public abstract class BlockBehaviours
{
    protected static Texture2D texture;
    protected static Texture2D topTexture;
    protected static float smoothness;

    protected static bool hasCollisions;

    protected static float speedMultiplier;
    protected static float jumpMultiplier;

    public BlockBehaviours(BlockBehaviours.Properties properties)
    {
        texture = properties.m_texture;
        topTexture = properties.m_topTexture;
        smoothness = properties.m_smoothness;

        hasCollisions = properties.m_hasCollisions;

        speedMultiplier = properties.m_speedMultiplier;
        jumpMultiplier = properties.m_jumpMultiplier;
    }

    public class Properties
    {
        public int m_id;

        public bool m_hasVoxelIcon = true;
        public Texture2D m_icon;

        public bool m_hasConnections = true;

        public bool m_hasTop = false;
   
        public string m_textureName;
        public string m_topTextureName;
        public string m_iconName;

        public Texture2D m_texture;
        public Texture2D m_topTexture;
        public float m_smoothness;

        public bool m_hasCollisions;

        public float m_speedMultiplier;
        public float m_jumpMultiplier;

        public float m_toughness;

        public bool m_hasOtherDrop = false;
        public Block m_otherdrop;

        public int m_energyValue = 0;

        public bool m_isTogglable = false;

        private Properties(string texture, string topTexture, bool hasTop)
        {
            this.m_hasTop = hasTop;
            this.m_iconName = texture;
            this.m_textureName = texture;
            this.m_topTextureName = topTexture;
        }

        public static BlockBehaviours.Properties of(string texture)
        {
            return new BlockBehaviours.Properties(texture, texture, false);
        }

        public static BlockBehaviours.Properties of(string texture, string topTexture)
        {
            return new BlockBehaviours.Properties(texture, topTexture, true);
        }

        public BlockBehaviours.Properties Toughness(float value)
        {
            this.m_toughness = value;
            return this;
        }

        //DROPPED BLOCK MUST BE CREATED BEFORE THIS BLOCK!
        public BlockBehaviours.Properties DropsOther(Block block)
        {
            m_hasOtherDrop = true;
            m_otherdrop = block;
            return this;
        }

        public BlockBehaviours.Properties SetIcon(string icon)
        {
            m_hasVoxelIcon = false;
            m_iconName = icon;
            return this;
        }

        public BlockBehaviours.Properties ContainsEnergy(int amount)
        {
            m_energyValue = amount;
            return this;
        }

        public BlockBehaviours.Properties IsTogglable()
        {
            m_isTogglable = true;
            return this;
        }

        public BlockBehaviours.Properties DisableTilingConnect()
        {
            m_hasConnections = false;
            return this;
        }
    }
}
