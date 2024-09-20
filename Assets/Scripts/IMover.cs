using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMover
{
    void SetProperties(Vector3 direction, float speed, Bounds bounds);
}
