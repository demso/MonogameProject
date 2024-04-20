using FarseerPhysics.Dynamics;
using FirstGame.Game.entyties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirstGame.Game.factories;
using FirstGame.Game.objects.bodies;
using FirstGame.Game.objects.bodies.player;
using Microsoft.Xna.Framework;
using Nez.Tiled;

namespace FirstGame.Game.objects.tiles
{
    public class Door : IInteractable, BodyData
    {
        public TmxLayerTile Cell;
        public TmxTilesetTile ClosedTile;
        public TmxTilesetTile OpenTile;
        public Filter ClosedFilter;
        public Filter OpenFilter;
        public Body PhysicalBody;
        public MasterScene GameState;
        public bool IsBoarded;
        public bool Peep;
        public bool IsOpen = false;

        public Door(TmxLayerTile cell, Body body)
        {
            GameState = MasterScene.Instance;
            this.Cell = cell;
            PhysicalBody = body;
            ClosedFilter = new Filter();
            OpenFilter = BodyResolver.CreateFilter((short)(Globals.NONE_CONTACT_FILTER | Globals.PLAYER_INTERACT_CONTACT_FILTER), ClosedFilter.categoryBits, ClosedFilter.groupIndex);
        }

        public void Open()
        {
            IsOpen = true;
            Filter.ApplyFilter(OpenFilter, PhysicalBody.FixtureList[0]);
            Cell.TilesetTile = OpenTile;
        }

        public void Close()
        {
            IsOpen = false;
            Filter.ApplyFilter(ClosedFilter, PhysicalBody.FixtureList[0]);
            Cell.TilesetTile = ClosedTile;
        }

        public void Toggle()
        {
            if (IsOpen)
                Close();
            else
                Open();
        }

        public void Board()
        {
            IsBoarded = true;
        }

        public void UnBoard()
        {
            IsBoarded = false;
        }

        public void Interact(Player player)
        {
            Toggle();
            //player.Body.Body.FixtureList[0].Refilter();
        }

        public TmxTilesetTile GetTile()
        {
            return Cell.TilesetTile;
        }

        public string GetName()
        {
            return GetTile().Properties["name"] as String;
        }

        public object GetData()
        {
            return this;
        }

        public Vector2 GetPosition()
        {
            return PhysicalBody.Position;
        }
    }
}
