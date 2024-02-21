/* Rights Reserved to Leland TL Carter of LethalLizard Studios ©2024
-- Last Change: 11/1/2023
*/

public class Block : BlockBehaviours
{
    public int ID
    {
        get
        {
            return m_properties.m_id;
        }
        set
        {
            m_properties.m_id = value;
        }
    }

    public bool isSolid
    {
        get
        {
            return ID != 0;
        }
    }

    public string m_name;
    public Properties m_properties;

    public Block(string name, Properties properties) : base(properties)
    {
        m_name = name;
        m_properties = properties;
    }
}
