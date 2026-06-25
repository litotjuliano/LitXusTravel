"use client";

import { SidebarProvider } from "@/context/SidebarContext";
import { ThemeProvider } from "@/context/ThemeContext";
import AppSidebar from "@/components/layout/AppSidebar";
import AppHeader from "@/components/layout/AppHeader";
import Backdrop from "@/components/layout/Backdrop";
import { SubscriptionBanner } from "@/components/dashboard/SubscriptionBanner";
import { useSubscription } from "@/lib/hooks/useSubscription";

export default function DashboardLayout({ children }: { children: React.ReactNode }) {
  const { subscription } = useSubscription();

  return (
    <ThemeProvider>
      <SidebarProvider>
        <div className="flex h-screen overflow-hidden bg-gray-50 dark:bg-gray-900">
          <AppSidebar />
          <Backdrop />
          <div className="flex flex-1 flex-col overflow-y-auto overflow-x-hidden">
            <AppHeader />
            <main className="flex-1 p-4 sm:p-6 lg:p-8">
              {subscription && (
                <div className="mb-4">
                  <SubscriptionBanner subscription={subscription} />
                </div>
              )}
              {children}
            </main>
          </div>
        </div>
      </SidebarProvider>
    </ThemeProvider>
  );
}
