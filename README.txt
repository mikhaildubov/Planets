3D Solar system simulator with planet rotation which is based on quaternion arithmetics.

The program is built as an XNA application for Windows Phone 7. It uses the rendering facilities of XNA, but implements 3D shapes collection and their rotation process on a low level through quaternions. 

Besides, the application demostrates some principles of building up Silverlight-like user interfaces purely in XNA (this approach has recently become of lower interest, as the possibilites of integrating XNA graphics into Silverlight applications were intriduced in WP7.5 SDK, but it still remains useful for certain purposes).



The project consists of three components:

1) DCL.Maths library, the basic quaternion arithmetic implementation;
2) DCL.Phone.Xna library, containig:
   * 3D primitives collection (Shpere, Ellipsoid et al.);
   * Pivot control implementation for use in XNA applications;
3) "Planets", the Windows Phone 7 application, visualizing the algorithms of the libraries.