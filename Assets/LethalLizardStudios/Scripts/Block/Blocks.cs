/* Rights Reserved to Leland TL Carter of LethalLizard Studios ©2024
-- Last Change: 11/19/2023
*/

public static class Blocks
{
    private const float EXTRA_SOFT = 0.15f;
    private const float SOFT = 0.3f;
    private const float HARD = 0.55f;
    private const float EXTRA_HARD = 0.8f;

    public static readonly int Count = 46;

    public static readonly Block Dirt = Registry.AddBlock(
        new Block("Dirt", BlockBehaviours.Properties.of("blocks/dirt.png")
        .Toughness(EXTRA_SOFT).ContainsEnergy(1)));

    public static readonly Block MixedSeeds = Registry.AddBlock(
        new Block("Mixed Seeds", BlockBehaviours.Properties.of("items/mixed_seeds.png")
        .Toughness(EXTRA_SOFT).SetIcon("items/mixed_seeds.png").ContainsEnergy(3).DisableTilingConnect()));

    public static readonly Block Grass = Registry.AddBlock(
        new Block("Grass", BlockBehaviours.Properties.of("blocks/grass.png", "blocks/grass_top.png")
        .Toughness(EXTRA_SOFT).DropsOther(MixedSeeds).ContainsEnergy(1).DisableTilingConnect().CannotBePlaced()));

    public static readonly Block Stone = Registry.AddBlock(
        new Block("Stone", BlockBehaviours.Properties.of("blocks/stone.png")
        .Toughness(SOFT).ContainsEnergy(3)));

    public static readonly Block Slate = Registry.AddBlock(
        new Block("Slate", BlockBehaviours.Properties.of("blocks/slate.png")
        .Toughness(HARD).ContainsEnergy(3)));

    public static readonly Block Unbreakium = Registry.AddBlock(
        new Block("Unbreakium", BlockBehaviours.Properties.of("blocks/unbreakium.png")
        .Toughness(99f)));

    public static readonly Block CoalOre = Registry.AddBlock(
        new Block("Coal Ore", BlockBehaviours.Properties.of("blocks/ore_coal.png")
        .Toughness(SOFT).SetIcon("items/raw_coal.png").DisableTilingConnect()));

    public static readonly Block CopperOre = Registry.AddBlock(
        new Block("Copper Ore", BlockBehaviours.Properties.of("blocks/ore_copper.png")
        .Toughness(SOFT).SetIcon("items/raw_copper.png").DisableTilingConnect()));

    public static readonly Block IronOre = Registry.AddBlock(
        new Block("Iron Ore", BlockBehaviours.Properties.of("blocks/ore_iron.png")
        .Toughness(HARD).SetIcon("items/raw_iron.png").DisableTilingConnect()));

    public static readonly Block Marble = Registry.AddBlock(
        new Block("Marble", BlockBehaviours.Properties.of("blocks/marble.png")
        .Toughness(SOFT)));

    public static readonly Block Hellrock = Registry.AddBlock(
        new Block("Hellrock", BlockBehaviours.Properties.of("blocks/hellrock.png")
        .Toughness(EXTRA_SOFT)));

    public static readonly Block DeadGrassStone = Registry.AddBlock(
        new Block("Dead Grass Stone", BlockBehaviours.Properties.of("blocks/grass_dead_stone.png", "blocks/grass_dead_top.png")
        .Toughness(SOFT).DropsOther(Stone).DisableTilingConnect()));

    public static readonly Block MagmaHellrock = Registry.AddBlock(
        new Block("Magma Hellrock", BlockBehaviours.Properties.of("blocks/magma_hellrock.png", "blocks/magma_top.png")
        .Toughness(EXTRA_SOFT).DropsOther(Hellrock).DisableTilingConnect()));

    public static readonly Block CinnabarOre = Registry.AddBlock(
        new Block("Cinnabar Ore", BlockBehaviours.Properties.of("blocks/ore_cinnabar.png")
        .Toughness(SOFT).SetIcon("items/raw_cinnabar.png").DisableTilingConnect()));

    public static readonly Block BasicCircuit = Registry.AddBlock(
        new Block("Basic Circuit", BlockBehaviours.Properties.of("blocks/circuit_basic.png")
        .Toughness(HARD).IsTogglable().DisableTilingConnect()));

    public static readonly Block Drill = Registry.AddBlock(
        new Block("Drill", BlockBehaviours.Properties.of("blocks/machine_drill.png")
        .Toughness(HARD).DisableTilingConnect()));

    public static readonly Block SpeedUpgrade = Registry.AddBlock(
        new Block("Speed Upgrade", BlockBehaviours.Properties.of("blocks/upgrade_speed.png")
        .Toughness(HARD).DisableTilingConnect()));

    public static readonly Block AdvancedCircuit = Registry.AddBlock(
        new Block("Advanced Circuit", BlockBehaviours.Properties.of("blocks/circuit_advanced.png")
        .Toughness(HARD).IsTogglable().DisableTilingConnect()));

    public static readonly Block Sand = Registry.AddBlock(
        new Block("Sand", BlockBehaviours.Properties.of("blocks/sand.png")
        .Toughness(EXTRA_SOFT)));

    public static readonly Block Slime = Registry.AddBlock(
        new Block("Slime", BlockBehaviours.Properties.of("blocks/slime.png")
        .Toughness(EXTRA_SOFT).IsTogglable().DisableTilingConnect()));

    public static readonly Block SlimyOre = Registry.AddBlock(
        new Block("Slimy Ore", BlockBehaviours.Properties.of("blocks/ore_slime.png")
        .Toughness(EXTRA_HARD).DropsOther(Slime).DisableTilingConnect()));

    public static readonly Block MarbleBrick = Registry.AddBlock(
        new Block("Marble Brick", BlockBehaviours.Properties.of("blocks/marble_brick.png")
        .Toughness(SOFT).DisableTilingConnect()));

    public static readonly Block MarbleColumn = Registry.AddBlock(
        new Block("Marble Column", BlockBehaviours.Properties.of("blocks/marble_column.png")
        .Toughness(SOFT).DisableTilingConnect()));

    public static readonly Block RawAmethyst = Registry.AddBlock(
        new Block("Raw Amethyst", BlockBehaviours.Properties.of("blocks/amethyst_raw.png")
        .Toughness(SOFT).DisableTilingConnect()));

    public static readonly Block SmoothAmethyst = Registry.AddBlock(
        new Block("Smooth Amethyst", BlockBehaviours.Properties.of("blocks/amethyst_smooth.png")
        .Toughness(SOFT).DisableTilingConnect()));

    public static readonly Block Copper = Registry.AddBlock(
        new Block("Copper", BlockBehaviours.Properties.of("blocks/copper_block.png")
        .Toughness(HARD).DisableTilingConnect()));

    public static readonly Block CopperBrick = Registry.AddBlock(
        new Block("Copper Brick", BlockBehaviours.Properties.of("blocks/copper_brick.png")
        .Toughness(HARD).DisableTilingConnect()));

    public static readonly Block Mud = Registry.AddBlock(
        new Block("Mud", BlockBehaviours.Properties.of("blocks/mud.png")
        .Toughness(EXTRA_SOFT)));

    public static readonly Block MudBrick = Registry.AddBlock(
        new Block("Mud Brick", BlockBehaviours.Properties.of("blocks/mud_brick.png")
        .Toughness(EXTRA_SOFT)));

    public static readonly Block Celadonite = Registry.AddBlock(
        new Block("Celadonite", BlockBehaviours.Properties.of("blocks/celadonite.png")
        .Toughness(HARD).DisableTilingConnect()));

    public static readonly Block Quartz = Registry.AddBlock(
        new Block("Quartz", BlockBehaviours.Properties.of("blocks/quartz.png")
        .Toughness(EXTRA_SOFT).DisableTilingConnect()));

    public static readonly Block Citrine = Registry.AddBlock(
        new Block("Citrine", BlockBehaviours.Properties.of("blocks/citrine_raw.png")
        .Toughness(SOFT).DisableTilingConnect()));

    public static readonly Block BlackrockBrick = Registry.AddBlock(
        new Block("Blackrock Brick", BlockBehaviours.Properties.of("blocks/blackrock_brick.png")
        .Toughness(HARD).DisableTilingConnect()));

    public static readonly Block BlackrockMossyBrick = Registry.AddBlock(
        new Block("Blackrock Mossy Brick", BlockBehaviours.Properties.of("blocks/blackrock_mossy_brick.png")
        .Toughness(HARD).DisableTilingConnect()));

    public static readonly Block BlackrockFloor = Registry.AddBlock(
        new Block("Blackrock Floor", BlockBehaviours.Properties.of("blocks/blackrock_floor.png", "blocks/floor_top.png")
        .Toughness(HARD).DisableTilingConnect()));

    public static readonly Block BlackrockMossyFloor = Registry.AddBlock(
        new Block("Blackrock Mossy Floor", BlockBehaviours.Properties.of("blocks/blackrock_grass.png", "blocks/grass_top.png")
        .Toughness(HARD).DisableTilingConnect()));

    public static readonly Block BlackrockPillar = Registry.AddBlock(
        new Block("Blackrock Pillar", BlockBehaviours.Properties.of("blocks/blackrock_pillar.png")
        .Toughness(HARD).DisableTilingConnect()));

    public static readonly Block Blackrock = Registry.AddBlock(
        new Block("Blackrock", BlockBehaviours.Properties.of("blocks/blackrock.png")
        .Toughness(HARD).DisableTilingConnect()));

    public static readonly Block BlackrockChisel = Registry.AddBlock(
        new Block("Blackrock Chisel", BlockBehaviours.Properties.of("blocks/blackrock_chisel.png")
        .Toughness(HARD).DisableTilingConnect()));

    public static readonly Block StonePillar = Registry.AddBlock(
        new Block("Stone Pillar", BlockBehaviours.Properties.of("blocks/stone_pillar.png")
        .Toughness(SOFT).DisableTilingConnect()));

    public static readonly Block Crystalarium = Registry.AddBlock(
        new Block("Crystalarium", BlockBehaviours.Properties.of("blocks/machine_crystalarium.png")
        .Toughness(HARD).IsTogglable().DisableTilingConnect()));

    public static readonly Block OpalOre = Registry.AddBlock(
        new Block("Opal Ore", BlockBehaviours.Properties.of("blocks/ore_opal.png")
        .Toughness(HARD).SetIcon("items/raw_opal.png").DisableTilingConnect()));

    public static readonly Block AluminumOre = Registry.AddBlock(
        new Block("Aluminum Ore", BlockBehaviours.Properties.of("blocks/ore_aluminum.png")
        .Toughness(SOFT).SetIcon("items/raw_aluminum.png").DisableTilingConnect()));

    public static readonly Block EnergyConverter = Registry.AddBlock(
        new Block("Energy Converter", BlockBehaviours.Properties.of("blocks/energy_converter.png")
        .Toughness(EXTRA_HARD).DisableTilingConnect()));

    public static readonly Block GoldBlock = Registry.AddBlock(
        new Block("Gold Block", BlockBehaviours.Properties.of("blocks/gold_block.png")
        .Toughness(HARD).DisableTilingConnect()));

    public static readonly Block IronBars = Registry.AddBlock(
        new Block("Iron Bars", BlockBehaviours.Properties.of("blocks/iron_bars.png")
        .Toughness(HARD).DisableTilingConnect()));

    public static readonly Block Planks = Registry.AddBlock(
        new Block("Planks", BlockBehaviours.Properties.of("blocks/planks.png")
        .Toughness(HARD).DisableTilingConnect()));

    public static readonly Block SpaceGradeCircuit = Registry.AddBlock(
        new Block("Space-Grade Circuit", BlockBehaviours.Properties.of("blocks/circuit_space_grade.png")
        .Toughness(HARD).IsTogglable().DisableTilingConnect()));

    public static readonly Block Sandstone = Registry.AddBlock(
        new Block("Sandstone", BlockBehaviours.Properties.of("blocks/sandstone.png")
        .Toughness(SOFT)));
}
