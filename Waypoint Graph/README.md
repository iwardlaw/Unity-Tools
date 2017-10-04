## Waypoint Graph ##

This waypoint graph exists to facilitate the movement of `GameObject`s in Unity among sets of points in generally unobstructed straight lines. It was created for use in _Fred's Happy Factory_, a team-based course project for Virtual Reality at the University of Arkansas at Little Rock in fall 2016, but it is meant to be generally applicable.

As with the other Unity tools I've posted here, the `Assets` directory contains all of the assets that will be loaded if you import `WaypointGraph01-Assets.unitypackage` into a Unity project; this includes the sample scene "WaypointGraph01", in `Assets/Scenes/`.

This scene contains two different spiders (both available free on the Asset Store) that move around on a small graph (five waypoints connected by eight links). The brown spider moves on a set path (0, 1, 2, 3, 4, 0, 5, repeat) and generates different audio clips depending on whether it's walking or stationary. The green spider moves about randomly and has had its `AudioSource` muted (because both spiders grumbling at once was too annoying). Note how they will not collide with each other; rather, the set-path brown spider waits for the green one to move if necessary, while the randomly moving green one never tries to move to a waypoint that the brown one either occupies or is headed toward.

![Waypoint graph demonstration. Two spiders move among the graph's points by following links.](../Images/WaypointGraph-01.jpg)

If you start a project from scratch, be sure to include all of the scripts: `AbstractGizmo`, `AbstractGizmoEditor`, `GraphGizmo`, `GraphTraverse`, `LinkGizmo`, `Pair`, `RAList`, `Vector3Eq`, `Waypoint`, `WaypointGraph`, and `WaypointLink`. `Vector3Eq` is a wrapper for `Vector3` that implements `IEquatable`, which was necessary to get it work in a `RAList` (random-access list); regarding this latter class, I built it to compensate for some flaw I perceived in the standard `List`, but I cannot remember what that was, and it may be entirely unnecessary. It is possible that I will replace `RAList`s with `List`s in the future, but for now, things are working fine.

* * *

### How to Build a Waypoint Graph ###

1. Create your waypoints as empty game objects, giving them coordinates and attaching the `Waypoint` and `GraphGizmo` scripts to each. Name them however you wish, but I suggest numbers or letters, which are easy to keep track of. I can't get the default values working correctly, so be sure to set `Active`; the only other values that do anything right now are `Min Idle Time` and `Max Idle Time`.

2. Create links as empty game objects. Coordinates do not matter. A naming convention as I've used ("Link[from]\_[to]") is probably the best for legibility. Drag waypoints in as `From Point` and `To Point`, and again, select `Active`. If you select `Directed`, the link is one-way, from `From Point` to `To Point`. `Weight` determines the likelihood that the link will be randomly selected for travel; it is not limited to the range [0, 1] but rather is given a probability equal to its value divided by the sum of all link weights for a given waypoint. Provide a `Movement Speed`, or a traversing object will be stuck. `Tolerance Weight` does nothing currently.

3. To ensure that the gizmos render correctly, make sure that all links are placed above all waypoints; the "Links" and "Waypoints" parent objects I have used in the sample scene are not necessary but help in this regard. White/light grey spheres represent active waypoints; dark grey are inactive; selected waypoints are blue. White lines are active undirected links, while yellow are active directed; grey lines are inactive undirected, and black are inactive directed; blue is selected. When a link is selected, its termini are also highlighted: blue for an undirected link, or red for a directed link's `From Point` and green for its `To Point`. This works best when you only select one link at a time.

4. Create an empty object to hold the graph and attach the `WaypointGraph` script. All links and waypoints in the graph must be children of this object. Declare a `Start Point` by dragging a waypoint into the appropriate slot in the Inspector. `End Point` is not currently used.

5. Add the `GraphTraverse` script to an object you want to move through the graph. Attach the graph object by dragging it into the `Graph` field in the Inspector. You may declare another `Start Point` here but do not have to; if you do, it will override the one in the `WaypointGraph`. Again, `End Point` does nothing at this time. Do not alter the other values: they were made public for the purposes of debugging, and I'm not ready to make them private again just yet.

6. Hit Play and watch your traversing object move around the graph. Either that, or something will break, and you'll yell at me.

* * *

#### UPDATE 16 Nov 2016 ####

You can now implement a set path using the correspondingly named public array of `Waypoint`s. Drag the waypoints in like you did for the links. The path is designed to loop, so be sure the last `Waypoint` in the array is connected to the start point. If you use a set path, the first point on that path overrides the `StartPoint`. You cannot force movement in the wrong way along a directed link.

Also, a link is now occupied when someone is on it. There should be no collisions/clipping unless two traversing objects are on two different but very close paths. It is possible to create a deadlock with the wrong combination of termini (nodes with only one link) and set paths; nevertheless, a random traverser (one without a set path) is set to time out after waiting for a link for 3 seconds and pick another one. The wait timeout is configurable as a public member in `GraphTraverse`. You can set the `Uses Occupied Links` flag in `GraphTraverse` if you want the object to ignore other objects on the graph.

Finally, while moving across a link, position is determined by how far an object has traveled on that link instead of how much time it has spent on it. In most cases, this won't matter, but it allows you to stop an object mid-link (with `Halt()`; use `PermitToMove()` to start it moving again; you can also manipulate the `stopped` member directly); this may help to avoid collisions if, say, the traverser is surrounded by a proximity trigger. Note that a side effect of this is that dynamically expanding or contracting the length of the link affects the object's movement speed, as length is calculated in `Start()`. So if you need to change the position of a `Waypoint`, be sure to call `RecalculateLength()` on every link attached to it.