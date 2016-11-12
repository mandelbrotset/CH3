using BulletSharp;
using CH3.GameObjects.DynamicObjects.Vehicles;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CH3.Physics
{
    public class WorldPhysics
    {

        const int ArraySizeX = 5, ArraySizeY = 5, ArraySizeZ = 5;

        ///scaling of the objects (0.1 = 20 centimeter boxes )
        const float StartPosX = -5;
        const float StartPosY = -5;
        const float StartPosZ = -3;
        
        public DiscreteDynamicsWorld World { get; set; }
        CollisionDispatcher dispatcher;
        DbvtBroadphase broadphase;
        List<CollisionShape> collisionShapes = new List<CollisionShape>();
        CollisionConfiguration collisionConf;

        public WorldPhysics()
        {
            collisionConf = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConf);

            broadphase = new DbvtBroadphase();
            World = new DiscreteDynamicsWorld(dispatcher, broadphase, null, collisionConf);
            World.Gravity = new Vector3(0, 0, -9.80665f);

            // create the ground
            CollisionShape groundShape = new StaticPlaneShape(new Vector3(0, 0, 1), 0);
            collisionShapes.Add(groundShape);
            CollisionObject ground = LocalCreateRigidBody(0, Matrix4.CreateTranslation(0, 0, 0), groundShape);
            ground.UserObject = "Ground";

            // create a few dynamic rigidbodies
 
        }


        public void AddRigidBody(GameObject g)
        {
            CollisionShape colShape = new BoxShape(g.hext);

            collisionShapes.Add(colShape);
            Vector3 localInertia = colShape.CalculateLocalInertia(g.mass);

            var rbInfo = new RigidBodyConstructionInfo(g.mass, null, colShape, localInertia);

            //var rbInfo = new RigidBodyConstructionInfo(0, null, colShape, new Vector3(0,0,0));


            // using motionstate is recommended, it provides interpolation capabilities
            // and only synchronizes 'active' objects
            Matrix4 m = Matrix4.CreateRotationX(-((float)Math.PI / 2));
            m = m * Matrix4.CreateRotationY(-((float)Math.PI));

            rbInfo.MotionState = new DefaultMotionState(m);

            g.body = new RigidBody(rbInfo);

            g.body.AngularVelocity = new Vector3(5, 2, 0);

            g.body.Translate(new Vector3(g.position.x, g.position.y, g.position.z) + new Vector3(g.offset.x, g.offset.y, g.offset.z));

            World.AddRigidBody(g.body);

            rbInfo.Dispose();
        }

        public void AddRigidBody(RigidBody body)
        {
            World.AddRigidBody(body);
        }


        public void AddVehicle(Car car)
        {

            CollisionShape chassisShape = new BoxShape(car.hext);
            collisionShapes.Add(chassisShape);

            CompoundShape compound = new CompoundShape();
            collisionShapes.Add(compound);

            //localTrans effectively shifts the center of mass with respect to the chassis
            Matrix4 localTrans = Matrix4.CreateTranslation(Vector3.UnitZ);
            compound.AddChildShape(localTrans, chassisShape);

            RigidBody carChassis = LocalCreateRigidBody(car.mass, Matrix4.Identity, compound);
            carChassis.UserObject = "Chassis";
            RaycastVehicle.VehicleTuning tuning = new RaycastVehicle.VehicleTuning();
            DefaultVehicleRaycaster vehicleRayCaster = new DefaultVehicleRaycaster(World);

            carChassis.ActivationState = ActivationState.DisableDeactivation;
            car.vehicle = new RaycastVehicle(tuning, carChassis, vehicleRayCaster);
            World.AddAction(car.vehicle);

            const float connectionHeight = 1.2f;
            bool isFrontWheel = true;

            const int rightIndex = 0;
            const int upIndex = 2;
            const int forwardIndex = 1;
            Vector3 wheelDirectionCS0 = new Vector3(0, 0, -1);
            Vector3 wheelAxleCS = new Vector3(-1, 0, 0);
            const float CUBE_HALF_EXTENTS = 1;
            const float wheelRadius = 0.7f;
            const float wheelWidth = 0.4f;
            const float wheelFriction = 20f;//BT_LARGE_FLOAT;
            const float suspensionStiffness = 10.0f;
            const float suspensionDamping = 2.3f;
            const float suspensionCompression = 4.4f;
            const float rollInfluence = 2.0f;//1.0f;
            const float suspensionRestLength = 0.6f;

            car.vehicle.SetCoordinateSystem(rightIndex, upIndex, forwardIndex);
            /*    
                      Vector3 connectionPointCS0 = new Vector3(CUBE_HALF_EXTENTS - (0.3f * wheelWidth), connectionHeight, 2 * CUBE_HALF_EXTENTS - wheelRadius);

                      car.vehicle.AddWheel(connectionPointCS0, wheelDirectionCS0, wheelAxleCS, suspensionRestLength, wheelRadius, tuning, isFrontWheel);

                      connectionPointCS0 = new Vector3(-CUBE_HALF_EXTENTS + (0.3f * wheelWidth), connectionHeight, 2 * CUBE_HALF_EXTENTS - wheelRadius);
                      car.vehicle.AddWheel(connectionPointCS0, wheelDirectionCS0, wheelAxleCS, suspensionRestLength, wheelRadius, tuning, isFrontWheel);

                      isFrontWheel = false;
                      connectionPointCS0 = new Vector3(-CUBE_HALF_EXTENTS + (0.3f * wheelWidth), connectionHeight, -2 * CUBE_HALF_EXTENTS + wheelRadius);
                      car.vehicle.AddWheel(connectionPointCS0, wheelDirectionCS0, wheelAxleCS, suspensionRestLength, wheelRadius, tuning, isFrontWheel);

                      connectionPointCS0 = new Vector3(CUBE_HALF_EXTENTS - (0.3f * wheelWidth), connectionHeight, -2 * CUBE_HALF_EXTENTS + wheelRadius);
                      car.vehicle.AddWheel(connectionPointCS0, wheelDirectionCS0, wheelAxleCS, suspensionRestLength, wheelRadius, tuning, isFrontWheel);
            */
            Vector3 connectionPointCS0 = new Vector3(CUBE_HALF_EXTENTS - (0.3f * wheelWidth), 2 * CUBE_HALF_EXTENTS - wheelRadius, connectionHeight);

            car.vehicle.AddWheel(connectionPointCS0, wheelDirectionCS0, wheelAxleCS, suspensionRestLength, wheelRadius, tuning, isFrontWheel);

            connectionPointCS0 = new Vector3(-CUBE_HALF_EXTENTS + (0.3f * wheelWidth),  2 * CUBE_HALF_EXTENTS - wheelRadius, connectionHeight);
            car.vehicle.AddWheel(connectionPointCS0, wheelDirectionCS0, wheelAxleCS, suspensionRestLength, wheelRadius, tuning, isFrontWheel);

            isFrontWheel = false;
            connectionPointCS0 = new Vector3(-CUBE_HALF_EXTENTS + (0.3f * wheelWidth),  -2 * CUBE_HALF_EXTENTS + wheelRadius, connectionHeight);
            car.vehicle.AddWheel(connectionPointCS0, wheelDirectionCS0, wheelAxleCS, suspensionRestLength, wheelRadius, tuning, isFrontWheel);

            connectionPointCS0 = new Vector3(CUBE_HALF_EXTENTS - (0.3f * wheelWidth), -2 * CUBE_HALF_EXTENTS + wheelRadius, connectionHeight);
            car.vehicle.AddWheel(connectionPointCS0, wheelDirectionCS0, wheelAxleCS, suspensionRestLength, wheelRadius, tuning, isFrontWheel);

            for (int i = 0; i < car.vehicle.NumWheels; i++)
            {
                WheelInfo wheel = car.vehicle.GetWheelInfo(i);
                wheel.SuspensionStiffness = suspensionStiffness;
                wheel.WheelsDampingRelaxation = suspensionDamping;
                wheel.WheelsDampingCompression = suspensionCompression;
                wheel.FrictionSlip = wheelFriction;
                wheel.RollInfluence = rollInfluence;
            }
            /*
            Matrix4 m = Matrix4.CreateRotationX(-((float)Math.PI / 2));
            m = m * Matrix4.CreateRotationY(-((float)Math.PI));

            m = m * Matrix4.CreateTranslation()
                */
            car.vehicle.RigidBody.Translate(new Vector3(car.position.x, car.position.y, car.position.z) + new Vector3(car.offset.x, car.offset.y, car.offset.z));

            car.body = car.vehicle.RigidBody;
        }

        public virtual void Update(float elapsedTime)
        {
            World.StepSimulation(elapsedTime);
        }

        public void Exit()
        {
            //remove/dispose constraints
            int i;
            for (i = World.NumConstraints - 1; i >= 0; i--)
            {
                TypedConstraint constraint = World.GetConstraint(i);
                World.RemoveConstraint(constraint);
                constraint.Dispose();
            }

            //remove the rigidbodies from the dynamics world and delete them
            for (i = World.NumCollisionObjects - 1; i >= 0; i--)
            {
                CollisionObject obj = World.CollisionObjectArray[i];
                RigidBody body = obj as RigidBody;
                if (body != null && body.MotionState != null)
                {
                    body.MotionState.Dispose();
                }
                World.RemoveCollisionObject(obj);
                obj.Dispose();
            }

            //delete collision shapes
            foreach (CollisionShape shape in collisionShapes)
                shape.Dispose();
            collisionShapes.Clear();

            World.Dispose();
            broadphase.Dispose();
            if (dispatcher != null)
            {
                dispatcher.Dispose();
            }
            collisionConf.Dispose();
        }

        public RigidBody LocalCreateRigidBody(float mass, Matrix4 startTransform, CollisionShape shape)
        {
            bool isDynamic = (mass != 0.0f);

            Vector3 localInertia = Vector3.Zero;
            if (isDynamic)
                shape.CalculateLocalInertia(mass, out localInertia);

            DefaultMotionState myMotionState = new DefaultMotionState(startTransform);

            RigidBodyConstructionInfo rbInfo = new RigidBodyConstructionInfo(mass, myMotionState, shape, localInertia);
            RigidBody body = new RigidBody(rbInfo);

            World.AddRigidBody(body);

            return body;
        }
    }
}
