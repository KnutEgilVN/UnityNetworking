using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Network.Models
{
    public class NetObject : MonoBehaviour
    {
        public Transform Transform;

        void Start()
        {
            Transform = transform;
        }
    }
}
