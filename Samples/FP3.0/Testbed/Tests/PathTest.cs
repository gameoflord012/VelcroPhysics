﻿using System;
using System.Collections.Generic;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using FarseerPhysics.TestBed.Framework;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.TestBed.Tests
{
    public class PathTest : Test
    {
        Body movingBody;
        Path path;

        private PathTest()
        {
            Body ground;
            {
                ground = BodyFactory.CreateBody(World);

                Vertices edge = PolygonTools.CreateEdge(new Vector2(-40.0f, 0.0f), new Vector2(40.0f, 0.0f));
                PolygonShape shape = new PolygonShape(edge, 0);
                ground.CreateFixture(shape);

                movingBody = BodyFactory.CreateBody(World);
                movingBody.Position = new Vector2(-25, 25);
                movingBody.BodyType = BodyType.Dynamic;
                movingBody.CreateFixture(new PolygonShape(PolygonTools.CreateRectangle(0.5f, 0.5f), 1));
                World.Add(movingBody);
            }

            {
                path = new Path();

                path.Add(new Vector2(0, 20));
                path.Add(new Vector2(5, 15));
                path.Add(new Vector2(20, 18));
                path.Add(new Vector2(15, 1));
                path.Add(new Vector2(-5, 14));

                path.Closed = true;

                Body body = new Body(World);
                body.BodyType = BodyType.Static;
                //body.CreateFixture(new PolygonShape(PolygonTools.CreateRectangle(0.5f, 0.5f, new Vector2(-0.5f, 0), 0), 1));
                Fixture f = body.CreateFixture(new CircleShape(0.25f, 1));

                PathFactory.EvenlyDistibuteShapesAlongPath(World, path, body, 100);

                Vector2 xform = new Vector2(0.5f, 0.5f);
                path.Scale(ref xform);
                xform = new Vector2(5, 5);
                path.Translate(ref xform);

                body = new Body(World);
                body.BodyType = BodyType.Dynamic;
                body.CreateFixture(new PolygonShape(PolygonTools.CreateRectangle(0.5f, 0.5f, new Vector2(-0.1f, 0), 0), 1));
                f = body.CreateFixture(new CircleShape(0.5f, 1));

                List<Body> bodies = PathFactory.EvenlyDistibuteShapesAlongPath(World, path, body, 20);

                JointFactory.AttachBodiesWithRevoluteJoint(World, bodies, new Vector2(0, 0.5f), new Vector2(0, -0.5f), true, true);

                xform = new Vector2(-25, 0);
                path.Translate(ref xform);

                body = new Body(World);
                body.BodyType = BodyType.Static;

                path.Closed = true;

                PathFactory.ConvertPathToEdges(World, path, body, 25);

                World.Add(body);

                xform = new Vector2(0, 25);
                path.Translate(ref xform);

                body = new Body(World);
                body.BodyType = BodyType.Dynamic;

                path.Closed = true;

                PathFactory.ConvertPathToPolygon(World, path, body, 1, 50);
                body.Position -= new Vector2(0, 10);
                World.Add(body);
            }
        }

        float time = 0;

        public override void Update(GameSettings settings, GameTime gameTime)
        {
            time += 0.01f;
            if (time > 1f)
                time = 0;
            
            PathFactory.MoveBodyOnPath(path, movingBody, time, 1f, 1f / 60f);
            
            base.Update(settings, gameTime);
        }

        internal static Test Create()
        {
            return new PathTest();
        }
    }
}