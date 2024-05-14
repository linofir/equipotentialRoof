# Equipotential Roof
* This project aims to create an equipotential using a provided mesh model, which determines the bleachersPoints as references.

## How to( need to check)
* Download Unity an create a scene.
* import a stair mesh model in the scene.
    * Configure the mesh, add meshFilter and mesh renderer components.
    * Add MeshManipulation script to the mesh
* Create new gameObject  and add Roof.cs stript to it.

## Demo

## PsedoCode
* [excalidraw](https://excalidraw.com/#json=tPOVgvDwu1H6V46uAMyxJ,Ag4BnnZ07LMFKz3Pc5FLoQ)

## ToDO
* Import a Mesh. ok
* Acess Data from Mesh. ok
* Recreate normals. ok
* Reorganize data classifying by face. ok
* Visualize triangles and normals methods defined by selected face.ok
* Calculate midpoint of stairs Thread for EquipotentialRoof. OK
    * Organize Threads. 
        * check the inclination of stairs.
        * if thre stais has no inclination in relation to plane XZ => determinate grop threads to the lowest to righest Y of vertex points.
            * agroup all triangles with the same average vertexY value.
            * sort each group by average.
        * else => determinate a line segment that passes throught all threshold points of the stais that has intersection with riser and thread.
    * Determine Thread points that define Thread dimentions, corner points.
        * Find the coordanates that are thresholds, defined by the two sides of the structure.
        * Determinate the corners that intersec with the previous and posterior riser.
    * Calculate midPoint of all Threads and crete bleachersPointsList.
* Use the midPoints of Threads as bleacherPoints. OK
    * find new direction roof reference, adapt bleachersDirection angle with Z, adapt CreateVectorWithAngle, Adapt FindIntersec.
* Refactoring for use cases.
    * Roof - Create a code independent of the Z-axis.
    * MeshManipulation - Reorganize data with greater independence from pre predefined angles.
    * MeshManipulation - Create a code that organize threads cosidering the inclination of the stairs. Correct the avarage number YAxis.
    * MeshManipulation - Improve the ordering of vertices of retangle threads.
* Create a mesh for the roof faces created.
* Develop a process for code updating using branches. Address issues with merging temporary and library files created automatically with Unity.