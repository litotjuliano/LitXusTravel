"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useEffect, useRef, useState } from "react";
import { Bell, Sun, Moon, LogOut, User, Settings, Search } from "lucide-react";
import { toast } from "sonner";
import { useSidebar } from "@/context/SidebarContext";
import { useTheme } from "@/context/ThemeContext";
import { Dropdown } from "@/components/ui/Dropdown";
import { DropdownItem } from "@/components/ui/DropdownItem";
import { NotificationDropdown } from "@/components/layout/NotificationDropdown";
import { useNotificationsContext } from "@/context/NotificationsContext";

interface UserInfo {
  email: string;
  firstName: string;
  lastName: string;
}

const AppHeader: React.FC = () => {
  const { isMobileOpen, toggleSidebar, toggleMobileSidebar } = useSidebar();
  const { theme, toggleTheme } = useTheme();
  const router = useRouter();

  const [user, setUser] = useState<UserInfo | null>(null);
  const [isUserDropdownOpen, setIsUserDropdownOpen] = useState(false);
  const [isNotifOpen, setIsNotifOpen] = useState(false);
  const userDropdownRef = useRef<HTMLDivElement>(null);
  const notifRef = useRef<HTMLDivElement>(null);

  const { notifications, unreadCount, loading, markRead, markAllRead } = useNotificationsContext();

  const loadUser = () => {
    const storedUserInfo = localStorage.getItem("user_info");
    if (storedUserInfo) {
      try {
        const info = JSON.parse(storedUserInfo);
        setUser({ email: info.email, firstName: info.firstName || "User", lastName: info.lastName || "" });
        return;
      } catch {}
    }
    const token = localStorage.getItem("litxus_token");
    if (token) {
      try {
        const decoded = JSON.parse(atob(token.split(".")[1]));
        setUser({ email: decoded.email, firstName: decoded.given_name || "User", lastName: decoded.family_name || "" });
      } catch {}
    }
  };

  useEffect(() => {
    loadUser();
    window.addEventListener("storage", loadUser);
    window.addEventListener("userInfoUpdated", loadUser);
    return () => {
      window.removeEventListener("storage", loadUser);
      window.removeEventListener("userInfoUpdated", loadUser);
    };
  }, []);

  const handleToggle = () => {
    if (window.innerWidth >= 1024) toggleSidebar();
    else toggleMobileSidebar();
  };

  const handleLogout = () => {
    localStorage.removeItem("litxus_token");
    localStorage.removeItem("user_info");
    toast.success("Logged out successfully");
    router.push("/auth/login");
  };

  const initials = user?.firstName?.[0]?.toUpperCase() || "A";

  return (
    <header className="sticky top-0 flex w-full bg-white border-b border-gray-200 z-99999 dark:border-gray-800 dark:bg-gray-900">
      <div className="flex items-center gap-2 grow px-4 lg:px-6 py-3">
        {/* Left: hamburger + mobile logo */}
        <div className="flex items-center gap-3 shrink-0">
          <button
            onClick={handleToggle}
            className="flex items-center justify-center w-10 h-10 text-gray-500 rounded-lg hover:bg-gray-100 dark:text-gray-400 dark:hover:bg-gray-800"
            aria-label="Toggle Sidebar"
          >
            {isMobileOpen ? (
              <svg width="20" height="20" viewBox="0 0 24 24" fill="none">
                <path fillRule="evenodd" clipRule="evenodd" d="M6.21967 7.28131C5.92678 6.98841 5.92678 6.51354 6.21967 6.22065C6.51256 5.92775 6.98744 5.92775 7.28033 6.22065L11.999 10.9393L16.7176 6.22078C17.0105 5.92789 17.4854 5.92788 17.7782 6.22078C18.0711 6.51367 18.0711 6.98855 17.7782 7.28144L13.0597 12L17.7782 16.7186C18.0711 17.0115 18.0711 17.4863 17.7782 17.7792C17.4854 18.0721 17.0105 18.0721 16.7176 17.7792L11.999 13.0607L7.28033 17.7794C6.98744 18.0722 6.51256 18.0722 6.21967 17.7794C5.92678 17.4865 5.92678 17.0116 6.21967 16.7187L10.9384 12L6.21967 7.28131Z" fill="currentColor" />
              </svg>
            ) : (
              <svg width="16" height="12" viewBox="0 0 16 12" fill="none">
                <path fillRule="evenodd" clipRule="evenodd" d="M0.583252 1C0.583252 0.585788 0.919038 0.25 1.33325 0.25H14.6666C15.0808 0.25 15.4166 0.585786 15.4166 1C15.4166 1.41421 15.0808 1.75 14.6666 1.75L1.33325 1.75C0.919038 1.75 0.583252 1.41422 0.583252 1ZM0.583252 11C0.583252 10.5858 0.919038 10.25 1.33325 10.25L14.6666 10.25C15.0808 10.25 15.4166 10.5858 15.4166 11C15.4166 11.4142 15.0808 11.75 14.6666 11.75L1.33325 11.75C0.919038 11.75 0.583252 11.4142 0.583252 11ZM1.33325 5.25C0.919038 5.25 0.583252 5.58579 0.583252 6C0.583252 6.41421 0.919038 6.75 1.33325 6.75L7.99992 6.75C8.41413 6.75 8.74992 6.41421 8.74992 6C8.74992 5.58579 8.41413 5.25 7.99992 5.25L1.33325 5.25Z" fill="currentColor" />
              </svg>
            )}
          </button>

          <Link href="/" className="lg:hidden">
            <span className="text-lg font-bold text-brand-500">LitXusTravel</span>
          </Link>
        </div>

        {/* Center: search */}
        <div className="hidden md:flex flex-1 max-w-sm mx-2 lg:mx-6">
          <div className="relative w-full">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 size-4 text-gray-400 pointer-events-none" />
            <input
              type="text"
              placeholder="Search or type command..."
              className="w-full pl-9 pr-14 py-2 text-sm rounded-lg border border-gray-200 dark:border-gray-800 bg-gray-50 dark:bg-gray-800 text-gray-900 dark:text-white placeholder:text-gray-400 focus:outline-none focus:ring-2 focus:ring-brand-500/30 transition-colors"
            />
            <span className="absolute right-3 top-1/2 -translate-y-1/2 text-[11px] text-gray-400 bg-gray-100 dark:bg-gray-700 border border-gray-200 dark:border-gray-700 px-1.5 py-0.5 rounded font-mono">⌘K</span>
          </div>
        </div>

        {/* Right: theme toggle + notifications + user */}
        <div className="flex items-center gap-2 ml-auto">
          {/* Dark mode */}
          <button
            onClick={toggleTheme}
            className="flex items-center justify-center w-10 h-10 text-gray-500 rounded-lg hover:bg-gray-100 dark:text-gray-400 dark:hover:bg-gray-800"
            aria-label="Toggle theme"
          >
            {theme === "dark" ? <Sun size={18} /> : <Moon size={18} />}
          </button>

          {/* Notifications */}
          <div className="relative" ref={notifRef}>
            <button
              onClick={() => setIsNotifOpen((o) => !o)}
              className="dropdown-toggle relative flex items-center justify-center w-10 h-10 text-gray-500 rounded-lg hover:bg-gray-100 dark:text-gray-400 dark:hover:bg-gray-800"
              aria-label="Notifications"
            >
              <Bell size={18} />
              {unreadCount > 0 && (
                <span className="absolute -top-0.5 -right-0.5 min-w-[18px] h-[18px] bg-brand-500 text-white text-[10px] font-bold rounded-full flex items-center justify-center px-1 leading-none">
                  {unreadCount > 99 ? "99+" : String(unreadCount).padStart(2, "0")}
                </span>
              )}
            </button>
            <NotificationDropdown
              isOpen={isNotifOpen}
              onClose={() => setIsNotifOpen(false)}
              notifications={notifications}
              unreadCount={unreadCount}
              loading={loading}
              onMarkRead={markRead}
              onMarkAllRead={markAllRead}
            />
          </div>

          {/* User dropdown */}
          <div className="relative" ref={userDropdownRef}>
            <button
              onClick={() => setIsUserDropdownOpen((o) => !o)}
              className="dropdown-toggle flex items-center gap-2.5 p-1.5 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800"
            >
              <div className="flex items-center justify-center w-9 h-9 rounded-full bg-brand-500 text-white text-sm font-semibold">
                {initials}
              </div>
              <div className="hidden sm:block text-left">
                <p className="text-sm font-medium text-gray-800 dark:text-white/90 leading-tight">
                  {user?.firstName || "User"}
                </p>
                <p className="text-xs text-gray-500 dark:text-gray-400">
                  {user?.email || "—"}
                </p>
              </div>
            </button>

            <Dropdown
              isOpen={isUserDropdownOpen}
              onClose={() => setIsUserDropdownOpen(false)}
              className="w-48 py-1"
            >
              <DropdownItem
                tag="button"
                onClick={() => { setIsUserDropdownOpen(false); router.push("/profile"); }}
                baseClassName="flex items-center gap-2 w-full text-left px-4 py-2.5 text-sm text-gray-700 hover:bg-gray-100 dark:text-gray-300 dark:hover:bg-white/5"
              >
                <User size={15} />
                Profile
              </DropdownItem>
              <DropdownItem
                tag="button"
                onClick={() => { setIsUserDropdownOpen(false); router.push("/settings"); }}
                baseClassName="flex items-center gap-2 w-full text-left px-4 py-2.5 text-sm text-gray-700 hover:bg-gray-100 dark:text-gray-300 dark:hover:bg-white/5"
              >
                <Settings size={15} />
                Settings
              </DropdownItem>
              <div className="my-1 border-t border-gray-100 dark:border-gray-800" />
              <DropdownItem
                tag="button"
                onClick={() => { setIsUserDropdownOpen(false); handleLogout(); }}
                baseClassName="flex items-center gap-2 w-full text-left px-4 py-2.5 text-sm text-error-600 hover:bg-error-50 dark:text-error-400 dark:hover:bg-error-500/10"
              >
                <LogOut size={15} />
                Logout
              </DropdownItem>
            </Dropdown>
          </div>
        </div>
      </div>
    </header>
  );
};

export default AppHeader;
