import { useEffect, useState } from 'react';
import { 
  BarChart3, 
  TrendingUp, 
  Users, 
  ShoppingCart,
  Eye,
  MousePointer
} from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { api } from '@/lib/api';

interface AnalyticsData {
  totalPageViews: number;
  totalClicks: number;
  conversionRate: number;
  abandonedCarts: number;
  topCategories: { name: string; views: number }[];
  dailyRevenue: { date: string; amount: number }[];
}

const AdminAnalytics = () => {
  const [data, setData] = useState<AnalyticsData>({
    totalPageViews: 0,
    totalClicks: 0,
    conversionRate: 0,
    abandonedCarts: 0,
    topCategories: [],
    dailyRevenue: [],
  });
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    fetchAnalytics();
  }, []);

  const fetchAnalytics = async () => {
    try {
      const { data, error } = await api.get<any>('/admin/analytics');
      if (error || !data) throw new Error(error || 'Failed to fetch analytics');
      
      setData({
        totalPageViews: data.orders?.last_7d || 0,
        totalClicks: data.orders?.last_30d || 0,
        conversionRate: data.conversion_rate || 0,
        abandonedCarts: data.abandoned_carts || 0,
        topCategories: data.top_categories?.map((c: any) => ({ name: c.category_name, views: c.total_quantity })) || [],
        dailyRevenue: [],
      });
    } catch (error) {
      console.error('Error fetching analytics:', error);
    }
    setIsLoading(false);
  };
        
  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-primary"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold">Analytics</h1>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <Card className="border-border">
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-muted-foreground">Page Views</p>
                <p className="text-2xl font-bold mt-1">{data.totalPageViews.toLocaleString()}</p>
              </div>
              <div className="w-12 h-12 rounded-xl bg-blue-500/10 flex items-center justify-center">
                <Eye className="w-6 h-6 text-blue-500" />
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="border-border">
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-muted-foreground">Total Clicks</p>
                <p className="text-2xl font-bold mt-1">{data.totalClicks.toLocaleString()}</p>
              </div>
              <div className="w-12 h-12 rounded-xl bg-green-500/10 flex items-center justify-center">
                <MousePointer className="w-6 h-6 text-green-500" />
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="border-border">
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-muted-foreground">Conversion Rate</p>
                <p className="text-2xl font-bold mt-1">{data.conversionRate.toFixed(1)}%</p>
              </div>
              <div className="w-12 h-12 rounded-xl bg-purple-500/10 flex items-center justify-center">
                <TrendingUp className="w-6 h-6 text-purple-500" />
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="border-border">
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-muted-foreground">Abandoned Carts</p>
                <p className="text-2xl font-bold mt-1">{data.abandonedCarts}</p>
              </div>
              <div className="w-12 h-12 rounded-xl bg-orange-500/10 flex items-center justify-center">
                <ShoppingCart className="w-6 h-6 text-orange-500" />
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Daily Revenue Chart */}
        <Card className="border-border">
          <CardHeader>
            <CardTitle className="text-lg flex items-center gap-2">
              <BarChart3 className="w-5 h-5" />
              Daily Revenue (Last 7 Days)
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {data.dailyRevenue.map((day) => (
                <div key={day.date} className="flex items-center gap-4">
                  <span className="text-sm text-muted-foreground w-24">
                    {new Date(day.date).toLocaleDateString('en-US', { weekday: 'short', month: 'short', day: 'numeric' })}
                  </span>
                  <div className="flex-1 h-8 bg-muted rounded-lg overflow-hidden">
                    <div
                      className="h-full gradient-primary transition-all duration-500"
                      style={{
                        width: `${Math.max(5, (day.amount / Math.max(...data.dailyRevenue.map(d => d.amount), 1)) * 100)}%`,
                      }}
                    />
                  </div>
                  <span className="font-semibold w-24 text-right">${day.amount.toFixed(2)}</span>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>

        {/* Top Categories */}
        <Card className="border-border">
          <CardHeader>
            <CardTitle className="text-lg flex items-center gap-2">
              <Users className="w-5 h-5" />
              Top Viewed Categories
            </CardTitle>
          </CardHeader>
          <CardContent>
            {data.topCategories.length === 0 ? (
              <p className="text-muted-foreground text-center py-8">
                No category data yet. Views will appear here as users browse.
              </p>
            ) : (
              <div className="space-y-4">
                {data.topCategories.map((category, index) => (
                  <div key={category.name} className="flex items-center gap-4">
                    <span className="w-6 h-6 rounded-full bg-primary/10 text-primary flex items-center justify-center text-sm font-bold">
                      {index + 1}
                    </span>
                    <span className="flex-1 font-medium">{category.name}</span>
                    <span className="text-muted-foreground">{category.views} views</span>
                  </div>
                ))}
              </div>
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  );
};

export default AdminAnalytics;
