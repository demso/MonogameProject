using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
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
        FSCollisionCircle fixture = (FSCollisionCircle) new FSCollisionCircle(5)
            .SetCollidesWith(Category.All)
            .SetCollisionCategories(Category.Cat1)
            .SetRestitution(0)
            .SetFriction(0.5f)
            .SetDensity(1);

        FSCollisionCircle filterFixture = (FSCollisionCircle)new FSCollisionCircle(15)
            .SetCollidesWith(Category.All)
            .SetCollisionCategories(Category.Cat1)
            .SetRestitution(0)
            .SetFriction(0f)
            .SetDensity(1)
            .SetIsSensor(true);

        Body.SetBodyType(BodyType.Dynamic);
        Body.SetFixedRotation(true);
        Body.SetMass(60f);
        Body.SetLinearDamping(12f);
        AddComponent(Body);

        Body.Body.UserData = this;

        AddComponent(fixture);
        AddComponent(filterFixture);

        Transform.SetScale(scale);
        Transform.SetPosition(350, 350);
    }
    
    public override void Kill()
    {
        base.Kill();
        Helper.Log("Player {Name} killed");
    }

    public override bool OnBeginContact(Fixture thisFixture, Fixture otherFixture, Contact contact)
    {
        
        return base.OnBeginContact(thisFixture, otherFixture, contact);
    }

    //public bool onColl(Fixture A, Fixture B, Contact contact)
    //{
    //    return true;
    //}

    public override void OnEndContact(Fixture thisFixture, Fixture otherFixture, Contact contact)
    {
        base.OnEndContact(thisFixture, otherFixture, contact);
    }

    public void Revive(){
        Hp = MaxHp;
        IsAlive = true;
        Helper.Log("Player {Name} revived");
    }
}