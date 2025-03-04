import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {CurrentComponent} from "./current/current.component";
import {GraphType} from "./shared/graph-type";
import {ChartComponent} from "./shared/components/chart.component";

const routes: Routes = [
    {
        path: '', redirectTo: '/current', pathMatch: 'full'
    },
    {
        path: 'current',
        component: CurrentComponent
    },
    {
        path: 'temperature-humidity',
        component: ChartComponent,
        data: {
            graphType: GraphType.Temperature
        }
    },
    {
        path: 'pressure',
        component: ChartComponent,
        data: {
            graphType: GraphType.Pressure
        }
    },
    {
        path: 'air-quality',
        component: ChartComponent,
        data: {
            graphType: GraphType.AirQuality
        }
    }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule {
}
