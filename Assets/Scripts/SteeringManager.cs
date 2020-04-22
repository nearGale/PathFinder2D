using System;
using UnityEngine;

namespace Application
{
    public interface IEntity
    {
        Vector2 GetVelocity();
        float GetMaxVelocity();
        Vector2 GetPosition();
        float GetMass();
        void SetPosition(Vector2 pos);
        void SetVelocity(Vector2 velocity);
    }
    public class SteeringManager
    {
        private float m_ForceSteer_Max = 1f;

        public IEntity host
        {
            get; private set;
        }
        public Vector2 Steering {
            get; private set;
        }
        public SteeringManager(IEntity host)
        {
            this.host = host;
            Steering = Vector2.zero;
        }

        public void Update()
        {
            Vector2 v = host.GetVelocity();
            Vector2 pos = host.GetPosition();
            var tempSteering = Steering;
            VectorHandler.Truncate(ref tempSteering, m_ForceSteer_Max);
            Steering = tempSteering;
            Steering *= 1 / host.GetMass();
 
            v = v + Steering;
            VectorHandler.Truncate(ref v, host.GetMaxVelocity());
 
            host.SetVelocity(v);
            host.SetPosition(pos + v);
        }
        public void Seek(Vector2 target, float slowingRadius = 1f)
        {
            Steering += DoSeek(target, slowingRadius);
        }
        private Vector2 DoSeek(Vector2 target, float slowingRadius = 1f){
            Vector2 force ;
            float distance;
 
           var  desired = target - host.GetPosition();
 
            distance = desired.magnitude;
            desired.Normalize();
 
            if (distance <= slowingRadius) {
                desired *= host.GetMaxVelocity() * distance/slowingRadius;
            } else {
                desired *= host.GetMaxVelocity();
            }
 
            force = desired - host.GetVelocity();
 
            return force;
        }
        
        
    }
}
