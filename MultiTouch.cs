/**
*
	Copyright (c) 2020 - 2021 Chay Palton

	Permission is hereby granted, free of charge, to any person obtaining
	a copy of this software and associated documentation files (the "Software"),
	to deal in the Software without restriction, including without limitation
	the rights to use, copy, modify, merge, publish, distribute, sublicense,
	and/or sell copies of the Software, and to permit persons to whom the Software
	is furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
	EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
	OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
	IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
	CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
	TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
	OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Godot;

/*
 * Mobile device multi-touch
 * NOTE: Can't do multi touch on Windows 10, as it does not seem to work correct with Godot.
 **
 * 09/06/21
 * Add rotation 
 */

public class MultiTouch : Node2D
{
    Label label1 = null;
    Label label2 = null;
    Label label3 = null;
    Label label4 = null;

    Line2D line2D = new Line2D();

    // Only handle two finger touching the screen;
    Vector2[] touches = new Vector2[2];
    Vector2[] touchesDrag = new Vector2[2];
    ColorRect[] colorRectFinger = new ColorRect[2];
    ColorRect colorRect = null;
    int touchCounter = 0;

    float distanceLast = 0;
    float angleLast = 0;

    bool isDrag = false;

    [Export]
    bool enableRotation = false;
    // filter out noise.
    [Export]
    float touchResolution = 2;
    [Export]
    float maxScale = 2;
    [Export]
    float minScale = 0.2f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        label1 = GetNode<Label>("Label1");
        label2 = GetNode<Label>("Label2");
        label3 = GetNode<Label>("Label3");
        label4 = GetNode<Label>("Label4");
        colorRect = GetNode<ColorRect>("ColorRect");

        label1.Text ="";
        label2.Text ="";
        label3.Text ="";
        label4.Text ="";
        
        touches[0] = new Vector2();
        touches[1] = new Vector2();
        touchesDrag[0] = new Vector2();
        touchesDrag[1] = new Vector2();
  
        colorRectFinger[0] =  GetNode<ColorRect>("ColorRectFinger1");
        colorRectFinger[1] =  GetNode<ColorRect>("ColorRectFinger2");

        line2D.DefaultColor = Colors.Red;
        line2D.Width = 8;
        line2D.AddPoint( new Vector2(0,0));
        line2D.AddPoint( new Vector2(0,0));
        line2D.Visible = false;

        AddChildBelowNode(label1,line2D);
    }

    public void UpdateScale(bool zoomIn, float diff)
    {
        float scale = 0;
        float zoomFactor = 0.02f;
        Vector2 vector = new Vector2();
    
        if(zoomIn)
        {
            scale = colorRect.RectScale.x - zoomFactor;
        } else {
            scale = colorRect.RectScale.x + zoomFactor;
        }

        scale = Mathf.Clamp(scale, minScale , maxScale);

        colorRect.RectScale = new Vector2(scale,scale);

        label3.Text = "Zoom direction: " + ((zoomIn)?"IN":"OUT") + " : scale" + vector;
    }

    public void UpdateRotation(float angle)
    {
        // Remember, you will always be rotatating around 
        // where ever the pivot is located. 

        float diff = (angle - angleLast);

        colorRect.RectRotation = colorRect.RectRotation + diff;

        label4.Text = "Degrees: " +  angle.ToString();
    }

    // Handle inputs ( make sure settings emulate touch is set.)
    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is InputEventScreenTouch touchEvent)
        {
            isDrag = false;
            /*
             * Keep track of number touches that are active.
             */
            if(touchEvent.Pressed)
            {
                touchCounter++;
            } else {
                touchCounter--;
            }

            /*
             * Only handle two touches, but also allow this 
             * code to work on Desktop (Windows) where only
             * touchEvent.Index 0-1 touch well be allowed. So effectively
             * one touch at a time.
             * Godot does not work well with Windows multi-touch (So mulit touch does not work well on windows) 
             */
            if(touchEvent.Index < 2)
            {
                if(touchEvent.Pressed)
                {
                    distanceLast = 0;
                    angleLast = 0;
        
                    touches[touchEvent.Index].x = touchEvent.Position.x;
                    touches[touchEvent.Index].y = touchEvent.Position.y;

                    touchesDrag[touchEvent.Index].x = touchEvent.Position.x;
                    touchesDrag[touchEvent.Index].y = touchEvent.Position.y;
                    
                    colorRectFinger[touchEvent.Index].SetPosition(
                        new Vector2(touches[touchEvent.Index].x - 50,
                                    touches[touchEvent.Index].y - 50));

                    colorRectFinger[touchEvent.Index].Visible = true;

                } else {
                   
                    colorRectFinger[touchEvent.Index].Visible = false;
                }
            } 
        } 
        else if (inputEvent is InputEventScreenDrag grapEvent)
        {
            /*
             * Drag means that the touch is still active, so just 
             * update the position
             */

            if(grapEvent.Index < 2)
            {
                touchesDrag[grapEvent.Index].x = grapEvent.Position.x;
                touchesDrag[grapEvent.Index].y = grapEvent.Position.y;

                colorRectFinger[grapEvent.Index].SetPosition(
                        new Vector2(grapEvent.Position.x - 50,
                                    grapEvent.Position.y - 50));
            }

            isDrag = true;
        }

        /*
        * Drag mean that the touch is still active, so just 
        * update the position
        */
        if(touchCounter == 2)
        {
            if(isDrag == true)
            {
                // Knowing the distance between two touches can be useful for some application.
                float distance = touchesDrag[0].DistanceTo(touchesDrag[1]);
                // ** Do what you will with this value.

                // Knowing the center can be useful for some application.
                Vector2 centerPos = (touchesDrag[0] + touchesDrag[1]) / 2;
                // ** Do what you will with this value.

                // Knowing the angle can be useful for some application.
                float angle  = Mathf.Atan2(touchesDrag[0].y - touchesDrag[1].y , touchesDrag[0].x - touchesDrag[1].x) * 180 / Mathf.Pi;
                // ** Do what you will with this value.

                GD.Print("ANGLE + " + angle);

                line2D.DefaultColor = Colors.Red;
              
                if( distanceLast != 0)
                {
                    float diff = (distance - distanceLast);
                    bool zoomIn = false;

                    // Only change zoom direction if its with in the touchResolution range
                    if (Mathf.Abs(diff) > touchResolution)
                    {
                        if(distance > distanceLast) 
                        {
                            line2D.DefaultColor = Colors.Green;
                            zoomIn = false;
                        } else {
                            line2D.DefaultColor = Colors.Blue;
                            zoomIn = true;
                        }

                        UpdateScale(zoomIn, diff); 
                        
                    }   

                } else {
                     label3.Text = "";
                }


                // Use the rotation
                if(angleLast != 0 && enableRotation)
                {
                    UpdateRotation(angle); 

                } else {
                     label4.Text = "";
                }
     
                distanceLast = distance;
                angleLast = angle;

                line2D.Visible = true;
                line2D.SetPointPosition(0, touchesDrag[0]);
                line2D.SetPointPosition(1, touchesDrag[1]);

                label2.Visible = true;
                label2.SetPosition(centerPos);
                label2.Text = "Distance =" + distance;
            } else {
                label3.Text = "";
            }

        } else {
            label3.Text = "";
            label2.Visible = false;
            line2D.Visible = false;
        }
        
        label1.Text = "Touch count =" + touchCounter;
    }
}
