//* This script uses for get data of body (body tracking).
//* If you do not use it, you cannot get tracking id, skeleton position and so on.
//* 2020/02/24/Mon.
//* Y.Akematsu @yoslab

namespace K4AdotNet.BodyTracking{
    public struct K4ABody{
        public bool isTracked;          // Whether it track or not
        public BodyId bodyId;           // Tracked Body's Id(0:Not Tracked,1~7:Tracked Id)
        public Skeleton skeleton;       // Skelton

        public Float2?[] convertedPos;  // Converted 2D-Scale(Color or Depth) Position
    }
}
