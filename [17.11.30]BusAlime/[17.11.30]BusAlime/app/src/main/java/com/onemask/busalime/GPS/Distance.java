package com.onemask.busalime.GPS;

import android.location.Location;

public class Distance {

    public double getDistance( double P1_longitude, double P1_latitude,
                               double P2_latitude, double P2_longitude ) {
        Location locationA = new Location("From");
        Location locationB = new Location("To");
        setLocate(locationA,P1_latitude,P1_longitude);
        setLocate(locationB,P2_latitude,P2_longitude);
        return (locationA.distanceTo(locationB));
    }


    public void setLocate ( Location location , double latitude, double longitude ) {
        location.setLatitude(latitude);
        location.setLongitude(longitude);
    }
}
