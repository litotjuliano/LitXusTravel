"use client";

import { useCallback, useEffect, useRef, useState } from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import {
  LayoutDashboard,
  Package,
  Users,
  BarChart3,
  CreditCard,
  Settings,
  ChevronDown,
  MoreHorizontal,
  Receipt,
  UserCog,
  Bell,
} from "lucide-react";
import { useSidebar } from "@/context/SidebarContext";
import { useNotificationsContext } from "@/context/NotificationsContext";

type NavItem = {
  name: string;
  icon: React.ReactNode;
  path?: string;
  subItems?: { name: string; path: string }[];
};

const navItems: NavItem[] = [
  { icon: <LayoutDashboard size={22} />, name: "Dashboard", path: "/" },
  { icon: <Package size={22} />, name: "Packages", path: "/packages" },
  { icon: <Users size={22} />, name: "Tenants", path: "/tenants" },
  { icon: <CreditCard size={22} />, name: "Subscriptions", path: "/subscriptions" },
  { icon: <Bell size={22} />, name: "Notifications", path: "/notifications" },
  { icon: <BarChart3 size={22} />, name: "Analytics", path: "/analytics" },
  { icon: <Receipt size={22} />, name: "Billing", path: "/billing" },
  { icon: <UserCog size={22} />, name: "Users", path: "/users" },
  { icon: <Settings size={22} />, name: "Settings", path: "/settings" },
];

const AppSidebar: React.FC = () => {
  const { isExpanded, isMobileOpen, isHovered, setIsHovered } = useSidebar();
  const pathname = usePathname();
  const { unreadCount } = useNotificationsContext();

  const [openSubmenu, setOpenSubmenu] = useState<{ index: number } | null>(null);
  const [subMenuHeight, setSubMenuHeight] = useState<Record<string, number>>({});
  const subMenuRefs = useRef<Record<string, HTMLDivElement | null>>({});

  const isActive = useCallback(
    (path: string) =>
      path === "/" ? pathname === "/" : pathname.startsWith(path),
    [pathname]
  );

  useEffect(() => {
    let matched = false;
    navItems.forEach((nav, index) => {
      if (nav.subItems?.some((s) => isActive(s.path))) {
        setOpenSubmenu({ index });
        matched = true;
      }
    });
    if (!matched) setOpenSubmenu(null);
  }, [pathname, isActive]);

  useEffect(() => {
    if (openSubmenu !== null) {
      const key = `main-${openSubmenu.index}`;
      if (subMenuRefs.current[key]) {
        setSubMenuHeight((prev) => ({
          ...prev,
          [key]: subMenuRefs.current[key]?.scrollHeight || 0,
        }));
      }
    }
  }, [openSubmenu]);

  const handleSubmenuToggle = (index: number) => {
    setOpenSubmenu((prev) =>
      prev?.index === index ? null : { index }
    );
  };

  const showLabel = isExpanded || isHovered || isMobileOpen;

  return (
    <aside
      className={`fixed mt-16 flex flex-col lg:mt-0 top-0 px-5 left-0 bg-gray-dark border-r border-gray-800 h-screen transition-all duration-300 ease-in-out z-50
        ${isExpanded || isMobileOpen ? "w-[290px]" : isHovered ? "w-[290px]" : "w-[90px]"}
        ${isMobileOpen ? "translate-x-0" : "-translate-x-full"}
        lg:translate-x-0`}
      onMouseEnter={() => !isExpanded && setIsHovered(true)}
      onMouseLeave={() => setIsHovered(false)}
    >
      {/* Logo */}
      <div className={`py-8 flex ${!isExpanded && !isHovered ? "lg:justify-center" : "justify-start"}`}>
        <Link href="/">
          {showLabel ? (
            <span className="text-xl font-bold text-brand-500">LitXusTravel</span>
          ) : (
            <span className="text-xl font-bold text-brand-500">LX</span>
          )}
        </Link>
      </div>

      {/* Nav */}
      <div className="flex flex-col overflow-y-auto duration-300 ease-linear no-scrollbar">
        <nav className="mb-6">
          <div className="flex flex-col gap-4">
            <div>
              <h2 className={`mb-4 text-xs uppercase flex leading-[20px] text-gray-500 ${!isExpanded && !isHovered ? "lg:justify-center" : "justify-start"}`}>
                {showLabel ? "Menu" : <MoreHorizontal className="size-6" />}
              </h2>
              <ul className="flex flex-col gap-4">
                {navItems.map((nav, index) => (
                  <li key={nav.name}>
                    {nav.subItems ? (
                      <button
                        onClick={() => handleSubmenuToggle(index)}
                        className={`menu-item group ${openSubmenu?.index === index ? "menu-item-active" : "menu-item-inactive"} cursor-pointer ${!isExpanded && !isHovered ? "lg:justify-center" : "lg:justify-start"}`}
                      >
                        <span className={`menu-item-icon-size ${openSubmenu?.index === index ? "menu-item-icon-active" : "menu-item-icon-inactive"}`}>
                          {nav.icon}
                        </span>
                        {showLabel && <span className="menu-item-text">{nav.name}</span>}
                        {showLabel && (
                          <ChevronDown className={`ml-auto w-5 h-5 transition-transform duration-200 ${openSubmenu?.index === index ? "rotate-180 text-brand-500" : ""}`} />
                        )}
                      </button>
                    ) : (
                      nav.path && (
                        <Link
                          href={nav.path}
                          className={`menu-item group ${isActive(nav.path) ? "menu-item-active" : "menu-item-inactive"}`}
                        >
                          <span className={`menu-item-icon-size relative ${isActive(nav.path) ? "menu-item-icon-active" : "menu-item-icon-inactive"}`}>
                            {nav.icon}
                            {nav.name === "Notifications" && unreadCount > 0 && (
                              <span className="absolute -top-1 -right-1 min-w-[16px] h-4 px-1 bg-brand-500 text-white text-[9px] font-bold rounded-full flex items-center justify-center leading-none">
                                {unreadCount > 99 ? "99+" : unreadCount}
                              </span>
                            )}
                          </span>
                          {showLabel && <span className="menu-item-text">{nav.name}</span>}
                          {showLabel && nav.name === "Notifications" && unreadCount > 0 && (
                            <span className="ml-auto min-w-[20px] h-5 px-1.5 bg-brand-500 text-white text-[10px] font-bold rounded-full flex items-center justify-center">
                              {unreadCount > 99 ? "99+" : unreadCount}
                            </span>
                          )}
                        </Link>
                      )
                    )}
                    {nav.subItems && showLabel && (
                      <div
                        ref={(el) => { subMenuRefs.current[`main-${index}`] = el; }}
                        className="overflow-hidden transition-all duration-300"
                        style={{ height: openSubmenu?.index === index ? `${subMenuHeight[`main-${index}`]}px` : "0px" }}
                      >
                        <ul className="mt-2 space-y-1 ml-9">
                          {nav.subItems.map((sub) => (
                            <li key={sub.name}>
                              <Link
                                href={sub.path}
                                className={`menu-dropdown-item ${isActive(sub.path) ? "menu-dropdown-item-active" : "menu-dropdown-item-inactive"}`}
                              >
                                {sub.name}
                              </Link>
                            </li>
                          ))}
                        </ul>
                      </div>
                    )}
                  </li>
                ))}
              </ul>
            </div>
          </div>
        </nav>
      </div>
    </aside>
  );
};

export default AppSidebar;
