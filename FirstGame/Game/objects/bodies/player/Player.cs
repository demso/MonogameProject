using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FirstGame.Game.components;
using FirstGame.Game.entyties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Farseer;
using Nez.Textures;

namespace FirstGame.Game.objects.bodies.player;

public partial class Player : GameEntity
{
    public float moveSpeed = 2.5f;
    private PlayerRenderer _spriteRenderer;
    private float scale = 1.8f;
    private List<FSRigidBody> _closeObjects;
    internal List<FSRigidBody> CloseObjects
    {
        get => _closeObjects;
    }

    public Body ClosestObject;

    private PlayerCollisionHandler _collisionHandler;

    internal Player(string name) : base(name)
    {
        _spriteRenderer = new PlayerRenderer(new Sprite(Texture2D.FromFile(Core.GraphicsDevice, "Content/assets/ClassicRPG_Sheet.png"),
            new Rectangle(16, 0, 16, 16)));

        SetTag(Globals.TAG_FOR_FIXED_UPDATE);

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
        FSCollisionCircle fixture = (FSCollisionCircle)new FSCollisionCircle(5)
            .SetCollidesWith(Category.All)
            .SetCollisionCategories((Category)Globals.PLAYER_CONTACT_FILTER)
            .SetRestitution(0)
            .SetFriction(0.5f)
            .SetDensity(1);

        FSCollisionCircle filterFixture = (FSCollisionCircle)new FSCollisionCircle(15)
            .SetCollidesWith(Category.All)
            .SetCollisionCategories((Category) Globals.PLAYER_INTERACT_CONTACT_FILTER)
            .SetRestitution(0)
            .SetFriction(0f)
            .SetDensity(1)
            .SetIsSensor(true);

        Body.SetBodyType(BodyType.Dynamic);
        Body.SetFixedRotation(true);
        Body.SetMass(60f);
        Body.SetLinearDamping(10);
        AddComponent(Body);

        Body.Body.UserData = this;

        _collisionHandler = new PlayerCollisionHandler(this);

        AddComponent(fixture);
        AddComponent(filterFixture);
        AddComponent(_collisionHandler);

        Transform.SetScale(scale);
        Transform.SetPosition(350, 350);
    }

    public override void Kill()
    {
        base.Kill();
        Helper.Log("Player {Name} killed");
    }

    //public override bool OnBeginContact(Fixture thisFixture, Fixture otherFixture, Contact contact)
    //{
    //    UpdateClosestObject();
    //    return true;
    //}

    //public override void OnEndContact(Fixture thisFixture, Fixture otherFixture, Contact contact)
    //{
    //    base.OnEndContact(thisFixture, otherFixture, contact);
    //}

    //internal void UpdateClosestObject()
    //{

    //}

    public void Revive()
    {
        Hp = MaxHp;
        IsAlive = true;
        Helper.Log("Player {Name} revived");
    }

    public override CollisionHandler<Player> GetCollisionHandler()
    {
        return _collisionHandler;
    }

}