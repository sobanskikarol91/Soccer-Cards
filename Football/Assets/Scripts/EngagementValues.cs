using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EngagementValues
{
    public static Engagment small = new Engagment(-0.01f, -0.1f);
    public static Engagment medium = new Engagment(-0.03f, 0f);
    public static Engagment huge = new Engagment(-0.05f, 0.1f);

    public static Engagment difference1 = new Engagment(0,-0.04f);
    public static Engagment difference2 = new Engagment(0,-0.06f);
    public static Engagment difference3 = new Engagment(0,-0.07f);


    public static Engagment CheckEngagement(EENGAGMENT _eengamentType)
    {
        if (_eengamentType == EENGAGMENT.SMALL) return small;
        else if (_eengamentType == EENGAGMENT.MEDIM) return medium;
        else if (_eengamentType == EENGAGMENT.DIFFERENCE_1) return difference1;
        else if (_eengamentType == EENGAGMENT.DIFFERENCE_2) return difference2;
        else if (_eengamentType == EENGAGMENT.DIFFERENCE_3) return difference3;
        else return huge;
    }
}
