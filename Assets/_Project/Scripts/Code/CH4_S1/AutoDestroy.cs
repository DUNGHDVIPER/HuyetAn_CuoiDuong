using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Assets._Project.Scripts.Code.CH4_S1
{
    public class AutoDestroy : MonoBehaviour
    {
        public float lifeTime = 0.5f;

        void Start()
        {
            Destroy(gameObject, lifeTime);
        }
    }
    }

