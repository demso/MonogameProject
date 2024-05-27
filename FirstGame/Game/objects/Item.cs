//using FarseerPhysics.Dynamics;
//using FirstGame.Game.factories;
//using FirstGame.Game.objects.bodies.player;
//using FirstGame.Game.objects;
//using Nez.UI;
//using Nez;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using FirstGame.Game.entyties;
//using Microsoft.Xna.Framework;
//using Nez.Tiled;

//namespace FirstGame.Game.objects
//{
//    public class Item : Entity, BodyData, IInteractable {
//    public TmxTilesetTile tile;
//    public Body physicalBody;
//    public Table mouseHandler;
//    public Item item;

//    public MasterScene MasterDcene;
//    public String tileName = "Item name.";
//    public String itemName = "10мм FMJ";
//    public String description = "First you must develop a Skin that implements all the widgets you plan to use in your layout. You can't use a widget if it doesn't have a valid style. Do this how you would usually develop a Skin in Scene Composer.";
//    public float spriteWidth = 0.7f;
//    public float spiteHeight = 0.7f;
//    GameObject GO;

//    public Item(TmxTilesetTile tile, String itemName)
//    {
//        this.tile = tile;
//        this.item = this;
//        if (!tile.Properties.TryGetValue("name", out tileName))
//            tileName = "no_name";
//        this.MasterDcene = MasterScene.Instance;
//        this.itemName = itemName;

//    mouseHandler = new Table();
//    mouseHandler.setSize(spriteWidth-0.1f,spiteHeight-0.1f);
//        mouseHandler.setTouchable(Touchable.enabled);
//        mouseHandler.addListener(new ClickListener()
//    {
//        @Override
//            public void enter(InputEvent event, float x, float y, int pointer, @Null Actor fromActor)
//        {
//            super.enter(event, x, y, pointer, fromActor);
//            gameState.hud.debugEntries.put(tileName + "_ClickListener", "Pointing at " + tileName + " at " + getPosition());
//            gameState.hud.showItemInfoWindow(item);
//        }

//        @Override
//            public void exit(InputEvent event, float x, float y, int pointer, @Null Actor toActor)
//        {
//            super.exit(event, x, y, pointer, toActor);
//            MasterDcene.hud.debugEntries.removeKey(tileName + "_ClickListener");
//            MasterDcene.hud.hideItemInfoWindow(item);
//        }
//    });

//        MasterDcene.gameStage.addActor(mouseHandler);

//        GO = new GameObject("bullet", false, Instance.unbox);
//    new SpriteBehaviour(GO, spriteWidth, spiteHeight, tile.getTextureRegion(), Globals.DEFAULT_RENDER_ORDER);
//}

//public Item(String tileName, String itemName)
//{
//    this(TileResolver.getTile(tileName), itemName);
//}

//public Body allocate(Vector2 position)
//{
//    physicalBody = BodyResolver.itemBody(position.x, position.y, this);
//    new Box2dBehaviour(physicalBody, GO);
//    GO.setEnabled(true);
//    mouseHandler.setPosition(getPosition().x - mouseHandler.getWidth() / 2f, getPosition().y - mouseHandler.getHeight() / 2f);
//    return physicalBody;
//}

//public void removeFromWorld()
//{
//    if (mouseHandler != null)
//    {
//        MasterDcene.gameStage.getActors().removeValue(mouseHandler, true);
//    }
//    if (physicalBody != null)
//    {
//        clearPhysicalBody();
//    }
//}

//public Body getPhysicalBody()
//{
//    return physicalBody;
//}

//public Vector2 getPosition()
//{
//    return physicalBody.getPosition();
//}

//public void clearPhysicalBody()
//{
//    GO.setEnabled(false);
//    physicalBody = null;
//    GO.destroy(GO.getBox2dBehaviour());
//}

//@Override
//    public String getName()
//{
//    return "item " + itemName;
//}

//@Override
//    public Object getData()
//{
//    return this;
//}

//@Override
//    public void interact(Player player)
//{
//    Instance.player.takeItem(this);
//}
//}
//}
