package com.memoming.busalime.Map;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.support.v4.app.ActivityCompat;
import android.support.v7.app.AppCompatActivity;
import com.memoming.busalime.R;

import com.google.android.gms.maps.CameraUpdateFactory;
import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.OnMapReadyCallback;
import com.google.android.gms.maps.SupportMapFragment;
import com.google.android.gms.maps.model.CameraPosition;
import com.google.android.gms.maps.model.LatLng;
import com.google.android.gms.maps.model.MarkerOptions;

public class MapLocation extends AppCompatActivity implements OnMapReadyCallback {
    private GoogleMap gMap;
    double currLatitude=0;  //현재 인텐트의 위도 값
    double currLongitude=0; //현재 인텐트의 경도 값
    String string="";
    LatLng place;

    public MapLocation(){
    }
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Intent intent = this.getIntent();
        setContentView(R.layout.activity_map);
        currLatitude=intent.getDoubleExtra("lat",-1);
        currLongitude=intent.getDoubleExtra("lon",-1);
        string=intent.getStringExtra("stop");

        place = new LatLng(currLatitude,currLongitude);
        SupportMapFragment mapFragment = (SupportMapFragment) getSupportFragmentManager().findFragmentById(R.id.map);
        mapFragment.getMapAsync(this);
    }
    @Override
    public void onMapReady(GoogleMap googleMap) {
        gMap = googleMap;
        gMap.getUiSettings().setZoomControlsEnabled(true);
        gMap.getUiSettings().setCompassEnabled(true);
        gMap.getUiSettings().setMapToolbarEnabled(true);

//         // LatLng : 위도 경도 잡아주는 객체
//      //   LatLng place = new LatLng(currLatitude, currLongitude);
//
//         // 쥐잉~ 하면서 카메라 줌
//         // 20 : 확대하는 정도
        CameraPosition cameraPosition = new CameraPosition.Builder().target(place).zoom(19).build();
//
        gMap.animateCamera(CameraUpdateFactory.newCameraPosition((cameraPosition)));
//
//         // 좌표에 Marker가 꽂히게
        gMap.addMarker(new MarkerOptions().position(place).title(string));
        // gMap.addMarker(new MarkerOptions().position(place).snippet(string));

        //   Toast.makeText(this, "lat: " + currLatitude + ", lon: " + currLongitude, Toast.LENGTH_SHORT).show();
        if (ActivityCompat.checkSelfPermission(this, android.Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED && ActivityCompat.checkSelfPermission(this, android.Manifest.permission.ACCESS_COARSE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
            // TODO: Consider calling
            //    ActivityCompat#requestPermissions
            // here to request the missing permissions, and then overriding
            //   public void onRequestPermissionsResult(int requestCode, String[] permissions,
            //                                          int[] grantResults)
            // to handle the case where the user grants the permission. See the documentation
            // for ActivityCompat#requestPermissions for more details.
            return;
        }
        gMap.setMyLocationEnabled(true);
    }
}
