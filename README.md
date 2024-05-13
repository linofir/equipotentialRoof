# Equipotential Roof
* This project intend to create a equipotential roof 

## PsedoCode
* https://excalidraw.com/#json=jVHkcWpJ5XVZiNht776Z3,0dE08mn3Nbq96aN91sZaVA

## ToDO
* Import a Mesh. ok
* Acess Data from Mesh. ok
* Recreate normals. ok
* Reorganize data classifying by face. ok
* Visualize triangles and normals methods defined by selected face.ok
* Calculate midpoint of stairs Thread for EquipotentialRoof.
    * Organize Threads. 
        * check the inclination of stairs.
        * if thre satais has no inclination in ralation to plane XZ => determinate grop threads to the lowest to righest Y of vertex points.
            * agroup all triangles with the same average vertexY value.
            * sort each group by average.
        * else => determinate a line segment that passes throught all threshold points of the stais that has intersection with riser and thread.
    * Determine Thread points that define Thread dimentions, corner points.
        * Find the coordanates that are thresholds, defined by the two sides of the structure.
        * Determinate the corners that intersec with the previous and posterior riser.
    * Calculate midPoint of all Threads and crete bleachersPointsList.
* Use the midPoints of Threads as bleacherPoints.