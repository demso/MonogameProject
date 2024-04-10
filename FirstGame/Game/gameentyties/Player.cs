using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using FirstGame.Game.components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Farseer;
using Nez.Textures;

namespace FirstGame.Game;

public class Player : GameEntity
{
    private BodySpriteRenderer _spriteRenderer;
    private float scale = 1.8f;
    private List<FSRigidBody> _closeObjects;
    internal List<FSRigidBody> CloseObjects
    {
        get => _closeObjects;
    }
    private FSRigidBody _closestObject;
    
    internal Player(string name) : base(name)
    {
        _spriteRenderer = new BodySpriteRenderer(new Sprite(Texture2D.FromFile(Core.GraphicsDevice, "Content/assets/ClassicRPG_Sheet.png"),
            new Rectangle(16, 0, 16, 16)));

        AddComponent(_spriteRenderer);
        AddComponent(new PlayerController());

        Hp = 10;
        MaxHp = Hp;
        EntityKind = Kind.Player;
        EntityFriendliness = Friendliness.Neutral;
    }

    internal void InitBody()
    {
        Body = new FSRigidBody();
        FSCollisionCircle fixture = new FSCollisionCircle(5);
        fixture.SetCollidesWith(Category.All);
        fixture.SetCollisionCategories(Category.Cat1);
        fixture.SetRestitution(0);
        fixture.SetFriction(0.5f);
        fixture.SetDensity(1);
        Body.SetBodyType(BodyType.Dynamic);
        Body.SetFixedRotation(true);
        Body.SetMass(60f);
        Body.SetLinearDamping(12f);
        AddComponent(Body);

        Body.Body.UserData = this;

        AddComponent(fixture);

        Transform.SetScale(scale);
        Transform.SetPosition(50, 50);
    }
    

    public override void Kill()
    {
        base.Kill();
        Helper.Log("Player {Name} killed");
    }
    
    public void Revive(){
        Hp = MaxHp;
        IsAlive = true;
        Helper.Log("Player {Name} revived");
    }
}