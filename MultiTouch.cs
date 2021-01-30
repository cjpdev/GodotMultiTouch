/**
*
	Copyright (c) 2020 Chay Palton

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
 */

public class MultiTouch : Node2D
{
    Label label1 = null;
    Label label2 = null;

    Line2D line2D = new Line2D();

    // Only handle two finger touching the screen;
    Vector2[] touches = new Vector2[2];
    ColorRect[] colorRectFinger = new ColorRect[2];
    int touchCounter = 0;
    float distanceTo = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        label1 = GetNode<Label>("Label1");
        label2 = GetNode<Label>("Label2");

        touches[0] = new Vector2();
        touches[1] = new Vector2();
  
        colorRectFinger[0] =  GetNode<ColorRect>("ColorRectFinger1");
        colorRectFinger[1] =  GetNode<ColorRect>("ColorRectFinger2");

        line2D.DefaultColor  = Colors.Red;
        line2D.AddPoint( new Vector2(0,0));
        line2D.AddPoint( new Vector2(0,0));
        line2D.Visible = false;

        AddChild(line2D);
    }

    // Handle inputs ( make sure settings emulate touch is set.)
    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is InputEventScreenTouch touchEvent)
        {
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
             * touchEvent.Index 0-1 touch well be allowed. So efectivly
             * one touch at a time.
             * Because Godot does not work well with Windows multi-touch. 
             */
            if(touchEvent.Index < 2)
            {
                if(touchEvent.Pressed)
                {
                    touches[touchEvent.Index].x = touchEvent.Position.x;
                    touches[touchEvent.Index].y = touchEvent.Position.y;
                    
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
             * Drag mean that the touch is still active, so just 
             * update the position
             */

            if(grapEvent.Index < 2)
            {
                touches[grapEvent.Index].x = grapEvent.Position.x;
                touches[grapEvent.Index].y = grapEvent.Position.y;

                colorRectFinger[grapEvent.Index].SetPosition(
                        new Vector2(grapEvent.Position.x - 50,
                                    grapEvent.Position.y - 50));
            }
        }

        /*
        * Drag mean that the touch is still active, so just 
        * update the position
        */
        if(touchCounter == 2)
        {
            line2D.Visible = true;
            line2D.SetPointPosition(0, touches[0]);
            line2D.SetPointPosition(1, touches[1]);

            distanceTo = touches[0].DistanceTo( touches[1]);

            Vector2 infoPos = (touches[0] + touches[0]) / 2;

            line2D.Visible = true;
            label2.SetPosition(infoPos);
            label2.Text = "Distance =" + distanceTo;

        } else {
            label2.Visible = false;
            line2D.Visible = false;
        }
        
        label1.Text = "Touch count =" + touchCounter;
    }
}
