package com.onemask.busalime.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.support.design.widget.NavigationView;
import android.support.v4.view.GravityCompat;
import android.support.v4.widget.DrawerLayout;
import android.support.v7.app.ActionBar;
import android.support.v7.app.ActionBarActivity;
import android.support.v7.widget.Toolbar;
import android.view.MenuItem;
import android.widget.Toast;

import com.soo.busalime.R;


public class MainActivity extends ActionBarActivity {



    private final long	FINSH_INTERVAL_TIME    = 2000;
    private long		backPressedTime        = 0;

    private DrawerLayout mDrawerLayout;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);
        ActionBar actionBar = getSupportActionBar();
        actionBar.setHomeAsUpIndicator(R.drawable.ic_menu);
        actionBar.setDisplayHomeAsUpEnabled(true);

        mDrawerLayout = (DrawerLayout) findViewById(R.id.drawer_layout);
        NavigationView navigationView = (NavigationView) findViewById(R.id.navigation_view);
        navigationView.setNavigationItemSelectedListener(new NavigationView.OnNavigationItemSelectedListener() {
            @Override
            public boolean onNavigationItemSelected(@NonNull MenuItem menuItem) {
                mDrawerLayout.closeDrawers();

                int id = menuItem.getItemId();

                switch (id) {
                    case R.id.bus_stop:
                        Intent intent1 = new Intent(MainActivity.this ,Tab1Activity.class );
                        startActivity(intent1);
                    //    Toast.makeText(MainActivity.this, menuItem.getTitle(), Toast.LENGTH_LONG).show();
                        break;

                    case R.id.bus_num:
                        Intent intent2 = new Intent(MainActivity.this ,Tab2Activity.class );
                        startActivity(intent2);

                    //    Toast.makeText(MainActivity.this, menuItem.getTitle(), Toast.LENGTH_LONG).show();
                        break;

                    case R.id.setting:
                        Intent intent3 = new Intent(MainActivity.this ,SettingActivity.class );
                        startActivity(intent3);

                     //   Toast.makeText(MainActivity.this, menuItem.getTitle(), Toast.LENGTH_LONG).show();
                        break;

                    case R.id.nav_sub_menu_item01:
                     //   Toast.makeText(MainActivity.this, menuItem.getTitle(), Toast.LENGTH_LONG).show();
                        break;

                    case R.id.nav_sub_menu_item02:
                     //   Toast.makeText(MainActivity.this, menuItem.getTitle(), Toast.LENGTH_LONG).show();
                        break;

                }
                return true;
            }

        });
    }
//
//    @Override
//    public boolean onCreateOptionsMenu(Menu menu) {
//        getMenuInflater().inflate(R.menu.menu, menu);
//        return true;
//    }









    public void onBackPressed() {
             long tempTime        = System.currentTimeMillis();
                long intervalTime    = tempTime - backPressedTime;
               if ( 0 <= intervalTime && FINSH_INTERVAL_TIME >= intervalTime ) {
                        super.onBackPressed();
                    }
               else {
                         backPressedTime = tempTime;
                         Toast.makeText(getApplicationContext(),"'뒤로'버튼을한번더누르시면종료됩니다.",Toast.LENGTH_SHORT).show();
                     }
            }






    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();

        switch (id) {
            case android.R.id.home:
                mDrawerLayout.openDrawer(GravityCompat.START);
                return true;
            case R.id.action_settings:
                return true;
        }

        return super.onOptionsItemSelected(item);
    }
}
//
//        btnStop = (Button) findViewById(R.id.btnStop);
//                 btnNum = (Button)findViewById(R.id.btnNum);
//
//                btnStop.setOnClickListener(new View.OnClickListener() {
//
//            @Override
//            public void onClick(View v) {
//                Intent intent = new Intent(MainActivity.this ,Tab1Activity.class );
//                startActivity(intent);
//            }
//        });
//
//
//
//        btnNum.setOnClickListener(new View.OnClickListener() {
//            @Override
//            public void onClick(View v) {
//                Intent intent = new Intent(MainActivity.this ,Tab2Activity.class );
//                startActivity(intent);
//            }
//        });

//        init(); // 각 변수 초기화
//        srchBtn.setOnClickListener(new View.OnClickListener() {
//            @Override
//            public void onClick(View v) {
//                if      (user_select.equals("정류소"))
//                    stationExec();
//                else if (user_select.equals("노선번호"))
//                    busNumberExec();
//            }
//        });
 // } // end of onCreate

//
//    private void init () {
//        //mSpinner        = (Spinner)     findViewById(R.id.spinner);
//       // rltList         = (ListView)    findViewById(R.id.rltList);
//        srchBtn         = (Button)      findViewById(R.id.srchBtn);
//        input_stNm      = (EditText)    findViewById(R.id.stName);
//        spinnerMenuList = new ArrayList<String> ( Arrays.asList ( "정류소", "노선번호" ) );
//        adapter         = new ArrayAdapter<String>(this
//                                ,android.R.layout.simple_spinner_dropdown_item
//                                ,spinnerMenuList);
//        mSpinner.setAdapter(adapter);
//        mSpinner.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
//            @Override
//            public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
//                user_select = (String) mSpinner.getSelectedItem();
//            }
//            @Override
//            public void onNothingSelected(AdapterView<?> parent) {
//                user_select = (String) mSpinner.getSelectedItem();
//            }
//        });
//    } // end of init

//
//    private void stationExec () {
//        try {
//            GetStationByName_Thread thread
//                    = new GetStationByName_Thread( input_stNm.getText().toString() );
//            thread.start();
//            thread.join();
//
//            rltList.setAdapter ( new StationAdaptor(getBaseContext(), thread.getStations()) );
//            rltList.setOnItemClickListener ( new StationClickListener() );
//        }
//        catch (InterruptedException e)          { e.printStackTrace(); }
//        catch (UnsupportedEncodingException e)  { e.printStackTrace(); }
//    } // end of stationExec
//
//
//    private void busNumberExec () {
//        try {
//            GetBusListByBusNumber_Thread thread
//                    = new GetBusListByBusNumber_Thread( input_stNm.getText().toString() );
//            thread.start();
//            thread.join();
//
//            rltList.setAdapter( new BusAdaptor(getBaseContext(), thread.getBusList()) );
//            rltList.setOnItemClickListener( new BusClickListener() );
//        }
//        catch (InterruptedException e) { e.printStackTrace(); }
//    } // end of busNumberExec

//} // end of class





